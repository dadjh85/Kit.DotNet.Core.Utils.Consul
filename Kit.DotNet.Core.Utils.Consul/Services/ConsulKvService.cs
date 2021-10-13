using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kit.DotNet.Core.Utils.Extensions;
using Kit.DotNet.Core.Utils.Models;
using System.Net;
using System.IO;
using System.Linq;
using System.Reflection;
using Kit.DotNet.Core.Utils.Consul.Models;

namespace Kit.DotNet.Core.Utils.Consul.Services
{
    public class ConsulKvService : IConsulKvService
    {
        private readonly IHttpClientFactory _consulClientFactory;

        #region Constanst

        public const string URL_KV_CONSUL = "/v1/kv/";

        #endregion

        public ConsulKvService(IHttpClientFactory consulClientFactory)
        {
            _consulClientFactory = consulClientFactory ?? throw new ArgumentNullException(nameof(consulClientFactory));
        }

        public async Task<bool> AddFileKv(ConsulConfigurationFile consulConfigurationFile, string environment)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            List<string> fileNames = ProccessFileNames(consulConfigurationFile, environment);

            bool isUploadAllFiles = true;

            List<string> urlFilesInConsul = await GetListKv(consulConfigurationFile.Address);

            foreach (var item in fileNames)
            {
                bool isFileExists = urlFilesInConsul.Any(x => x.Trim() == item.Trim());

                if (consulConfigurationFile.ReloadConfigurationAllStartup || (!consulConfigurationFile.ReloadConfigurationAllStartup && !isFileExists))
                {
                    Response<string> response =  await UploadFile(consulConfigurationFile.Address, item, File.ReadAllText($"{appPath}/{item}"));

                    bool result = Convert.ToBoolean(response.Entity);

                    if (!result)
                        isUploadAllFiles = result;
                }
            }

            return isUploadAllFiles;
        }


        public async Task<List<string>> GetListKv(string urlConsul)
        {
            Response<List<string>> result = await CreateConsulClient(urlConsul).GetAsync<List<string>>(new RequestParameters { Url = $"{URL_KV_CONSUL}?keys"});

            ValidateResponse(result);

            return result.Entity;
        }


        #region Private methods

        private async Task<Response<string>> UploadFile(string urlConsul, string fileName, string contentFile)
        {
            Response<string> response = await CreateConsulClient(urlConsul + URL_KV_CONSUL + fileName).PutAsync<string>(
                    new RequestParameters
                    {
                        HttpContent = contentFile.ToStringContent()
                    });

            ValidateResponse(response);

            return response;
        }

        private List<string> ProccessFileNames(ConsulConfigurationFile consulConfigurationFile, string environment)
        {
            if(consulConfigurationFile.UrlFiles == null)
            {
                return GetFileName(consulConfigurationFile.UploadEnvironmentFile, consulConfigurationFile.UrlFile, environment);
            }
            else
            {
                List<string> files = new List<string>();
                foreach(var item in consulConfigurationFile.UrlFiles)
                {
                    files.AddRange(GetFileName(consulConfigurationFile.UploadEnvironmentFile, item, environment));
                }

                return files;
            }
        }

        private List<string> GetFileName(bool uploadEnvironmentFile, string urlFile, string environment)
        {
            if (uploadEnvironmentFile)
            {
                List<string> partialName = urlFile.SplitToList('.');

                return new List<string>
                {
                    urlFile,
                    $"{partialName.First()}.{environment}.{partialName.LastOrDefault()}"
                };
            }
            else
                return new List<string> { urlFile };
        }

        private void ValidateResponse<T>(Response<T> response) where T : class
        {
            if (response.HttpResponseMessage.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("no connection could be established with consul");
        }

        private HttpClient CreateConsulClient(string urlConsul)
        {
            HttpClient consulClient = _consulClientFactory.CreateClient();
            consulClient.BaseAddress = new Uri(urlConsul);
            return consulClient;
        }

        #endregion
    }
}
