using Consul;
using FluentAssertions;
using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Consul.Services.ConsulClientService;
using Kit.DotNet.Core.Utils.Models;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Kit.DotNet.Core.Utils.Consul.GlobalConfiguration;
using Xunit;

namespace Tests.Kit.DotNet.Core.Utils.Consul.Tests
{
    [Collection("Sequential")]
    public class ConsulClientServiceTest
    {
        #region Properties

        private IConsulClientService _consulClientService;
        private readonly ConsulClient _client;
        private readonly string _prefix;
        private readonly string _appPath;

        #endregion

        #region Constructor

        public ConsulClientServiceTest()
        {
            _consulClientService = CreateIConsulClientService();
            _client = new ConsulClient();
            _prefix = ResetCreatedKvConsulAttribute.PREFIX_CONSUL_TEST;
            _appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        #endregion

        [Fact]
        public void ConsulClientServic_without_consulClientFactory()
        {
            Assert.Throws<ArgumentNullException>(() => _consulClientService = new ConsulClientService(null));
        }

        #region Method GetListKv

        [Fact]
        [ResetCreatedKvConsul]
        public async Task GetListKv_when_reading_all_existing_key()
        {
            await UploadFilesToConsul(GetAllFilesName());

            HttpClient client = _consulClientService.CreateConsulClient(_client.Config.Address.OriginalString);

            Response<List<string>> kvConsul = await _consulClientService.GetListKv(client);

            kvConsul.Entity.Where(c => c.Contains(_prefix)).ToList().Count.Should().Be(4);

        }

        [Fact]
        public async Task GetListKv_when_non_existing_keys()
        {
            HttpClient client = _consulClientService.CreateConsulClient(_client.Config.Address.OriginalString);

            Response<List<string>> kvConsul = await _consulClientService.GetListKv(client);

            kvConsul.Entity.Where(c => c.Contains(_prefix)).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task GetListKv_with_consul_call_KO()
        {
            Mock<IHttpClientFactory> consulclientFactory = new Mock<IHttpClientFactory>();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{_client.Config.Address.OriginalString}/v1/kv/?keys").Respond(HttpStatusCode.NotFound);

            consulclientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockHttp));

            _consulClientService = new ConsulClientService(consulclientFactory.Object);
            HttpClient client = _consulClientService.CreateConsulClient(_client.Config.Address.OriginalString);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _consulClientService.GetListKv(client));
        }

        [Fact]
        public async Task GetListKv_when_get_url_files_consul_return_badrequest()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{_client.Config.Address.OriginalString}/v1/kv/?keys").Respond(HttpStatusCode.BadRequest);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _consulClientService.GetListKv(new HttpClient(mockHttp)));
        }

        #endregion

        #region Private Methods

        private KVPair Pair(string key, string value) 
            => new KVPair(_prefix + key) { Value = Encoding.UTF8.GetBytes(value) };

        private List<string> GetAllFilesName()
            => new List<string> 
               { 
                    TestsConstants.FILE_NAME_OPTION1,
                    TestsConstants.FILE_NAME_OPTION1_DEVELOPMENT,
                    TestsConstants.FILE_NAME_OPTION2,
                    TestsConstants.FILE_NAME_OPTION2_DEVELOPMENT 
               };

        private async Task UploadFilesToConsul(List<string> files)
        {

            foreach (var item in files)
            {
                string contentFile = File.ReadAllText($"{_appPath}/optionsfiles/{item}");
                await _client.KV.Put(Pair(item, contentFile));
            }
        }

        private IConsulClientService CreateIConsulClientService()
        {
            Mock<IHttpClientFactory> consulclientFactory = new Mock<IHttpClientFactory>();

            consulclientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            return new ConsulClientService(consulclientFactory.Object);
        }

        #endregion
    }
}
