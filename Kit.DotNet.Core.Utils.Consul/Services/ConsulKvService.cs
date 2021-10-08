using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kit.DotNet.Core.Utils.Extensions;

namespace Kit.DotNet.Core.Utils.Consul.Services
{
    public class ConsulKvService : IConsulKvService
    {
        private readonly IHttpClientFactory _consulClientFactory;
        private readonly IConfiguration _configuration;

        public ConsulKvService(IHttpClientFactory consulClientFactory, IConfiguration configuration)
        {
            _consulClientFactory = consulClientFactory ?? throw new ArgumentNullException(nameof(consulClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<string>> GetListKv(string urlConsul)
        {
            HttpClient consulClient = _consulClientFactory.CreateClient();
            consulClient.BaseAddress = new Uri(urlConsul);

            List<string> result = await consulClient.GetAsync<List<string>>("/v1/kv/?keys");

            if (result == null)
                throw new InvalidOperationException("no connection could be established with consul");

            return null;
        }
    }
}
