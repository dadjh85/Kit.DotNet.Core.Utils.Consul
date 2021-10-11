using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kit.DotNet.Core.Utils.Extensions;
using Kit.DotNet.Core.Utils.Models;
using System.Net;

namespace Kit.DotNet.Core.Utils.Consul.Services
{
    public class ConsulKvService : IConsulKvService
    {
        private readonly IHttpClientFactory _consulClientFactory;

        public ConsulKvService(IHttpClientFactory consulClientFactory)
        {
            _consulClientFactory = consulClientFactory ?? throw new ArgumentNullException(nameof(consulClientFactory));
        }

        public async Task<List<string>> GetListKv(string urlConsul)
        {
            HttpClient consulClient = _consulClientFactory.CreateClient();
            consulClient.BaseAddress = new Uri(urlConsul);

            Response<List<string>> result = await consulClient.GetAsync<List<string>>(new RequestParameters { Url = "/v1/kv/?keys"});

            if (result.HttpResponseMessage.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("no connection could be established with consul");

            return result.Entity;
        }
    }
}
