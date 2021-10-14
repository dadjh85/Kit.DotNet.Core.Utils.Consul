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
    /// <summary>
    /// Service that performs configuration file management in consul
    /// </summary>
    public class ConsulKvService : IConsulKvService
    {
        /// <summary>
        /// Object allowing api-rest calls to be made to the consul api
        /// </summary>
        private readonly IHttpClientFactory _consulClientFactory;

        #region Constanst

        /// <summary>
        /// constant with the basic path of the configuration of consul
        /// </summary>
        public const string URL_KV_CONSUL = "/v1/kv/";

        #endregion

        /// <summary>
        /// constructors's ConsulKvService 
        /// </summary>
        /// <param name="consulClientFactory">object allowing api-rest calls to be made to the consul api</param>
        public ConsulKvService(IHttpClientFactory consulClientFactory)
        {
            _consulClientFactory = consulClientFactory ?? throw new ArgumentNullException(nameof(consulClientFactory));
        }

        /// <summary>
        /// method for uploading a file or a list of files to the consul server.
        /// </summary>
        /// <param name="consulConfigurationFile">object with the configuration options for file uploading</param>
        /// <param name="environment">the .NET Core application environment</param>
        /// <returns>a boolean with the result of execution</returns>
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

        /// <summary>
        /// gets the list of file names uploaded to the consul server.
        /// </summary>
        /// <param name="urlConsul">the url of the consul server</param>
        /// <returns>a list of strings with the name of all files in consul</returns>
        public async Task<List<string>> GetListKv(string urlConsul)
        {
            Response<List<string>> result = await CreateConsulClient(urlConsul).GetAsync<List<string>>(new RequestParameters { Url = $"{URL_KV_CONSUL}?keys"});

            ValidateResponse(result);

            return result.Entity;
        }


        #region Private methods

        /// <summary>
        /// method that makes a put call to the consul api, which allows a file to be uploaded to the server
        /// </summary>
        /// <param name="urlConsul">the url of the consul server</param>
        /// <param name="fileName">the name of file</param>
        /// <param name="contentFile">the content of file</param>
        /// <returns>the response http and a string with the result of execution</returns>
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

        /// <summary>
        /// processes the name of the configuration files obtaining the configuration file per environment
        /// </summary>
        /// <param name="consulConfigurationFile">object with the configuration options for file uploading</param>
        /// <param name="environment">the .NET Core application environment</param>
        /// <returns>a list of string with all files to process</returns>
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

        /// <summary>
        /// method for get the name of the files to process
        /// </summary>
        /// <param name="uploadEnvironmentFile">a bool with the option to obtain the environment file</param>
        /// <param name="urlFile">the url of file to process in consul server</param>
        /// <param name="environment">the .NET Core application environment</param>
        /// <returns>a list of string with the names of files to process</returns>
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

        /// <summary>
        /// method for validate the response of the call api-rest of consul server
        /// </summary>
        /// <typeparam name="T">a generic object to process</typeparam>
        /// <param name="response">the result of execution</param>
        private void ValidateResponse<T>(Response<T> response) where T : class
        {
            if (response.HttpResponseMessage.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("no connection could be established with consul");
        }

        /// <summary>
        /// method for create a object HttpClient for call to consul server
        /// </summary>
        /// <param name="urlConsul">the url of consul server</param>
        /// <returns>a object HttpClient</returns>
        private HttpClient CreateConsulClient(string urlConsul)
        {
            HttpClient consulClient = _consulClientFactory.CreateClient();
            consulClient.BaseAddress = new Uri(urlConsul);
            return consulClient;
        }

        #endregion
    }
}
