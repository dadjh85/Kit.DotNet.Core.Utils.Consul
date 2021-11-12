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
            Response<List<string>> result = await client.GetAsync<List<string>>(new RequestParameters { Url = $"{URL_KV_CONSUL}?keys" });

            ValidateResponse(result);

            return result;
        }

        public HttpClient CreateConsulClient(string urlConsul)
        {
            HttpClient consulClient = _consulClientFactory.CreateClient("ApiConsul");

            if (urlConsul != null)
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

        #endregion
    }
}
