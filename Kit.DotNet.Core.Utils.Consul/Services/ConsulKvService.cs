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

        public async Task<bool> AddFileKv(string urlConsul, string environment = "Development", string urlFile = "appsettings.json", bool uploadEnvironmentFile = true)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            List<string> fileNames = ProccessFileNames(urlFile, environment, uploadEnvironmentFile);

            bool isUploadAllFiles = true;

            foreach(var item in fileNames)
            {
                string contentFile = File.ReadAllText($"{appPath}/{item}");

                Response<string> response = await CreateConsulClient(urlConsul + URL_KV_CONSUL + item).PutAsync<string>(
                new RequestParameters 
                {
                    HttpContent = contentFile.ToStringContent() 
                });

                ValidateResponse(response);

                bool result = Convert.ToBoolean(response.Entity);

                if (!result)
                    isUploadAllFiles = result;
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

        private List<string> ProccessFileNames(string urlFile, string environment, bool uploadEnvironmentFile)
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
