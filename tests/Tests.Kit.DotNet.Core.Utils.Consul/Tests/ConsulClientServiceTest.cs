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
using Tests.Kit.DotNet.Core.Utils.Consul.GlobalConfiguration.Tests;
using Tests.Tests.Infrastructure.GlobalConfiguration;
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
        public async Task GetListKv_when_client_is_null()
        {
            await UploadFilesToConsul(GetAllFilesName());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _consulClientService.GetListKv(null));
        }

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

        #region Method UploadFile

        [Fact]
        [ResetCreatedKvConsul]
        public async Task UploadFile_when_file_is_upload_consul()
        {

            HttpClient client = _consulClientService.CreateConsulClient(_client.Config.Address.OriginalString);

            ConsulConfigurationFile consulConfigurationFile = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                RelativeRouteFileConsul = $"/{_prefix}"
            };

            Response<string> result = await _consulClientService.UploadFile(client, 
                                                                            consulConfigurationFile, 
                                                                            "test.json", 
                                                                            File.ReadAllText($"{_appPath}/Optionsfiles/{GetAllFilesName().First()}"));

            result.HttpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Entity.Should().Be("true");
        }

        [Theory]
        [InlineData(null, "test")]
        [InlineData("test.json", null)]
        [InlineData(null, null)]
        public async Task UploadFile_when_fileName_or_contentFile_is_null(string fileName, string contentFile)
        {
            HttpClient client = _consulClientService.CreateConsulClient(_client.Config.Address.OriginalString);

            ConsulConfigurationFile consulConfigurationFile = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                RelativeRouteFileConsul = $"/{_prefix}"
            };

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _consulClientService.UploadFile(client, consulConfigurationFile, fileName, contentFile));
        }

        [Fact]
        public async Task UploadFile_when_client_is_null()
        {
            ConsulConfigurationFile consulConfigurationFile = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                RelativeRouteFileConsul = $"/{_prefix}"
            };

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _consulClientService.UploadFile(null, 
                                                                                                              consulConfigurationFile, 
                                                                                                              "test.json",
                                                                                                              File.ReadAllText($"{_appPath}/Optionsfiles/{GetAllFilesName().First()}")));
        }

        [Fact]
        public async Task UploadFile_when_consulConfigurationFile_is_null()
        {
            HttpClient client = _consulClientService.CreateConsulClient(_client.Config.Address.OriginalString);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _consulClientService.UploadFile(client,
                                                                                                              null,
                                                                                                              "test.json",
                                                                                                              File.ReadAllText($"{_appPath}/Optionsfiles/{GetAllFilesName().First()}")));
        }


        #endregion

        #region Method CreateConsulClient

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateConsulClient_when_urlConsul_is_null_or_empty(string url)
        {
            Assert.Throws<ArgumentNullException>(() => _consulClientService.CreateConsulClient(url));
        }

        [Fact]
        public void CreateConsulClient_when_url_is_ok()
        {
            HttpClient client = _consulClientService.CreateConsulClient("http://localhost");
            client.Should().NotBeNull();
            client.BaseAddress.Should().Be("http://localhost");
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
                string contentFile = File.ReadAllText($"{_appPath}/Optionsfiles/{item}");
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
