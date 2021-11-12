using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Extensions;
using Kit.DotNet.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Consul.Services.ConsulClientService
{
    public class ConsulClientService : IConsulClientService
    {

        #region Constanst

        /// <summary>
        /// constant with the basic path of the configuration of consul
        /// </summary>
        public const string URL_KV_CONSUL = "/v1/kv/";

        #endregion

        #region Properties

        private readonly IHttpClientFactory _consulClientFactory;

        #endregion

        #region Constructors

        public ConsulClientService(IHttpClientFactory consulClientFactory)
        {
            _consulClientFactory = consulClientFactory ?? throw new ArgumentNullException(nameof(consulClientFactory));
        }

        #endregion

        public async Task<Response<string>> UploadFile(HttpClient client, ConsulConfigurationFile consulConfigurationFile, string fileName, string contentFile)
        {
            ValidateUploadFileRequest(client, consulConfigurationFile, fileName, contentFile);

            Response<string> response = await client.PutAsync<string>(
                    new RequestParameters
                    {
                        Url = consulConfigurationFile.Address + URL_KV_CONSUL + consulConfigurationFile.RelativeRouteFileConsul + fileName,
                        HttpContent = contentFile.ToStringContent()
                    });

            ValidateResponse(response);

            return response;
        }

        public async Task<Response<List<string>>> GetListKv(HttpClient client)
        {
            ValidateRequest(client);

            Response<List<string>> result = await client.GetAsync<List<string>>(new RequestParameters { Url = $"{URL_KV_CONSUL}?keys" });

            ValidateResponse(result);

            return result;
        }

        public HttpClient CreateConsulClient(string urlConsul)
        {
            if (string.IsNullOrEmpty(urlConsul))
                throw new ArgumentNullException(nameof(urlConsul), "can't be null");

            HttpClient consulClient = _consulClientFactory.CreateClient("ApiConsul");

            consulClient.BaseAddress = new Uri(urlConsul);

            return consulClient;
        }

        #region Private Methods

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
        /// validate the request of the consulserver
        /// </summary>
        /// <param name="client">the client for conet with consul</param>
        private void ValidateRequest(HttpClient client)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client), "can't be null");
        }

        /// <summary>
        /// Validate the the request for upload the file in consul server
        /// </summary>
        /// <param name="client">the client for conet with consul</param>
        /// <param name="consulConfigurationFile">a object ConsulConfigurationFile with the confituration to upload files</param>
        /// <param name="fileName">the name of file</param>
        /// <param name="contentFile">the content of file</param>
        private void ValidateUploadFileRequest(HttpClient client, ConsulConfigurationFile consulConfigurationFile, string fileName, string contentFile)
        {
            ValidateRequest(client);
            if (consulConfigurationFile == null)
                throw new ArgumentNullException(nameof(consulConfigurationFile), "can't be null");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName), "can't be null");
            if(string.IsNullOrEmpty(contentFile)) 
                throw new ArgumentNullException(nameof(contentFile), "can't be null");
        }

        #endregion
    }
}
