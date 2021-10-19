using Consul;
using FluentAssertions;
using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Consul.Services;
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
using Kit.DotNet.Core.Utils.Extensions;

namespace Tests.Kit.DotNet.Core.Utils.Consul.Tests
{
    public class ConsulKvServiceTest
    {
        #region Properties

        private IConsulKvService _consulKvService;
        private readonly ConsulClient _client;
        private readonly string _prefix;
        private readonly string _appPath;

        #endregion

        #region Constansts

        private const string FILE_NAME_OPTION1 = "options1.json";
        private const string FILE_NAME_OPTION1_DEVELOPMENT = "options1.Development.json";
        private const string FILE_NAME_APPSETTINGS = "appsettings.json";
        private const string FILE_NAME_APPSETTINGS_DEVELOPMENT = "appsettings.Development.json";
        private const string FILE_NAME_OPTION2 = "options2.json";
        private const string FILE_NAME_OPTION2_DEVELOPMENT = "options2.Development.json";
        private const string PATH_FILE_OPTION1 = "optionsfiles/" + FILE_NAME_OPTION1;
        private const string PATH_FILE_OPTION1_DEVELOPMENT = "optionsfiles/" + FILE_NAME_OPTION1_DEVELOPMENT;
        private const string PATH_FILE_OPTION2 = "optionsfiles/" + FILE_NAME_OPTION2;
        private const string PATH_FILE_OPTION2_DEVELOPMENT = "optionsfiles/" + FILE_NAME_OPTION2_DEVELOPMENT;

        #endregion


        #region Constructor

        public ConsulKvServiceTest()
        {
            _consulKvService = CreateIConsulKvService();
            _client = new ConsulClient();
            _prefix = ResetCreatedKvConsulAttribute.PREFIX_CONSUL_TEST;
            _appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        #endregion

        [Fact]
        public void ConsulKvService_without_consulClientFactory()
        {
            Assert.Throws<ArgumentNullException>(() => _consulKvService = new ConsulKvService(null));
        }

        #region Method GetListKv

        [Fact]
        [ResetCreatedKvConsul]
        public async Task GetListKv_when_reading_all_existing_key()
        {
            await UploadFilesToConsul(GetAllFilesName());

            HttpClient client = _consulKvService.CreateConsulClient(_client.Config.Address.OriginalString);

            List<string> kvConsul = await _consulKvService.GetListKv(client);

            kvConsul.Where(c => c.Contains(_prefix)).ToList().Count.Should().Be(4);

        }

        [Fact]
        public async Task GetListKv_when_non_existing_keys()
        {
            HttpClient client = _consulKvService.CreateConsulClient(_client.Config.Address.OriginalString);

            List<string> kvConsul = await _consulKvService.GetListKv(client);

            kvConsul.Where(c => c.Contains(_prefix)).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task GetListKv_with_consul_call_KO()
        {
            Mock<IHttpClientFactory> consulclientFactory = new Mock<IHttpClientFactory>();

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{_client.Config.Address.OriginalString}/v1/kv/?keys").Respond(HttpStatusCode.NotFound);

            consulclientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockHttp));

            _consulKvService = new ConsulKvService(consulclientFactory.Object);

            HttpClient client = _consulKvService.CreateConsulClient(_client.Config.Address.OriginalString);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _consulKvService.GetListKv(client));
        }

        #endregion

        #region Method 

        #region Private Methods AddFileKv

        [Fact]
        [ResetCreatedKvConsul]
        public async Task AddFileKv_with_urlFile_and_environment()
        {
            var configurationFiles = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                UrlFile = PATH_FILE_OPTION1,
                RelativeRouteFileConsul = $"/{_prefix}"
            };

            bool result = await _consulKvService.AddFileKv(configurationFiles, "Development");
            result.Should().BeTrue();

            QueryResult<string[]> consulKeyOptions1 = await _client.KV.Keys($"/{_prefix}/{PATH_FILE_OPTION1}");
            consulKeyOptions1.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{PATH_FILE_OPTION1}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{PATH_FILE_OPTION1}"));

            QueryResult<string[]> consulKeyOptions1Development = await _client.KV.Keys($"/{_prefix}/{PATH_FILE_OPTION1_DEVELOPMENT}");
            consulKeyOptions1Development.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{PATH_FILE_OPTION1_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                               .Should()
                                                                                               .Be(File.ReadAllText($"{_appPath}/{PATH_FILE_OPTION1_DEVELOPMENT}"));
        }

        [Fact]
        [ResetCreatedKvConsul]
        public async Task AddFileKv_without_environment()
        {
            var configurationFiles = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                UrlFile = PATH_FILE_OPTION1,
                RelativeRouteFileConsul = $"/{_prefix}",
                UploadEnvironmentFile = false
            };

            bool result = await _consulKvService.AddFileKv(configurationFiles, null);
            result.Should().BeTrue();

            QueryResult<string[]> consulKeyOptions1 = await _client.KV.Keys($"/{_prefix}/{PATH_FILE_OPTION1}");
            consulKeyOptions1.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{PATH_FILE_OPTION1}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{PATH_FILE_OPTION1}"));
        }

        [Fact]
        [ResetCreatedKvConsul]
        public async Task AddFileKv_without_urlFile()
        {
            var configurationFiles = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                RelativeRouteFileConsul = $"/{_prefix}"
            };

            bool result = await _consulKvService.AddFileKv(configurationFiles, "Development");
            result.Should().BeTrue();

            QueryResult<string[]> consulKeyAppsettings = await _client.KV.Keys($"/{_prefix}/{FILE_NAME_APPSETTINGS}");
            consulKeyAppsettings.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{FILE_NAME_APPSETTINGS}").Result.Response.Value.BytesToString()
                                                                                       .Should()
                                                                                       .Be(File.ReadAllText($"{_appPath}/{FILE_NAME_APPSETTINGS}"));

            QueryResult<string[]> consulKeyAppsettingsDevelopment = await _client.KV.Keys($"/{_prefix}/{FILE_NAME_APPSETTINGS_DEVELOPMENT}");
            consulKeyAppsettingsDevelopment.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{FILE_NAME_APPSETTINGS_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                                   .Should()
                                                                                                   .Be(File.ReadAllText($"{_appPath}/{FILE_NAME_APPSETTINGS_DEVELOPMENT}"));
        }

        [Fact]
        [ResetCreatedKvConsul]
        public async Task AddFileKv_with_list_urlFiles()
        {
            var configurationFiles = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                RelativeRouteFileConsul = $"/{_prefix}",
                UrlFiles = new List<string>()
                {
                    FILE_NAME_APPSETTINGS,
                    PATH_FILE_OPTION1,
                    PATH_FILE_OPTION2
                }
            };

            bool result = await _consulKvService.AddFileKv(configurationFiles, "Development");
            result.Should().BeTrue();

            QueryResult<string[]> consulKeyAppsettings = await _client.KV.Keys($"/{_prefix}/{FILE_NAME_APPSETTINGS}");
            consulKeyAppsettings.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{FILE_NAME_APPSETTINGS}").Result.Response.Value.BytesToString()
                                                                                       .Should()
                                                                                       .Be(File.ReadAllText($"{_appPath}/{FILE_NAME_APPSETTINGS}"));

            QueryResult<string[]> consulKeyAppsettingsDevelopment = await _client.KV.Keys($"/{_prefix}/{FILE_NAME_APPSETTINGS_DEVELOPMENT}");
            consulKeyAppsettingsDevelopment.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{FILE_NAME_APPSETTINGS_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                                   .Should()
                                                                                                   .Be(File.ReadAllText($"{_appPath}/{FILE_NAME_APPSETTINGS_DEVELOPMENT}"));

            QueryResult<string[]> consulKeyOptions1 = await _client.KV.Keys($"/{_prefix}/{PATH_FILE_OPTION1}");
            consulKeyOptions1.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{PATH_FILE_OPTION1}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{PATH_FILE_OPTION1}"));

            QueryResult<string[]> consulKeyOptions1Development = await _client.KV.Keys($"/{_prefix}/{PATH_FILE_OPTION1_DEVELOPMENT}");
            consulKeyOptions1Development.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{PATH_FILE_OPTION1_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                               .Should()
                                                                                               .Be(File.ReadAllText($"{_appPath}/{PATH_FILE_OPTION1_DEVELOPMENT}"));

            QueryResult<string[]> consulKeyOptions2 = await _client.KV.Keys($"/{_prefix}/{PATH_FILE_OPTION2}");
            consulKeyOptions2.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{PATH_FILE_OPTION2}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{PATH_FILE_OPTION2}"));

            QueryResult<string[]> consulKeyOptions2Development = await _client.KV.Keys($"/{_prefix}/{PATH_FILE_OPTION2_DEVELOPMENT}");
            consulKeyOptions2Development.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{PATH_FILE_OPTION2_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                               .Should()
                                                                                               .Be(File.ReadAllText($"{_appPath}/{PATH_FILE_OPTION2_DEVELOPMENT}"));


        }

        #endregion

        private KVPair Pair(string key, string value) => new KVPair(_prefix + key) { Value = Encoding.UTF8.GetBytes(value) };

        private List<string> GetAllFilesName()
            => new List<string> { FILE_NAME_OPTION1, FILE_NAME_OPTION1_DEVELOPMENT, FILE_NAME_OPTION2, FILE_NAME_OPTION2_DEVELOPMENT };

        private async Task UploadFilesToConsul(List<string> files)
        {

            foreach (var item in files)
            {
                string contentFile = File.ReadAllText($"{_appPath}/optionsfiles/{item}");
                await _client.KV.Put(Pair(item, contentFile));
            }
        }
        
        private IConsulKvService CreateIConsulKvService()
        {
            Mock<IHttpClientFactory> consulclientFactory = new Mock<IHttpClientFactory>();

            consulclientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            return new ConsulKvService(consulclientFactory.Object);
        }

        #endregion
    }
}
