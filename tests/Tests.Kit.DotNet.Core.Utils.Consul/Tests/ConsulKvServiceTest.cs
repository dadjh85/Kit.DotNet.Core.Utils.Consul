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
using Kit.DotNet.Core.Utils.Consul.Services.ConsulKvService;
using Kit.DotNet.Core.Utils.Consul.Services.ConsulClientService;
using Kit.DotNet.Core.Utils.Models;

namespace Tests.Kit.DotNet.Core.Utils.Consul.Tests
{
    [Collection("Sequential")]
    public class ConsulKvServiceTest
    {
        #region Properties

        private IConsulKvService _consulKvService;
        private readonly ConsulClient _client;
        private readonly string _prefix;
        private readonly string _appPath;

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

        #region Method AddFileKv

        [Fact]
        [ResetCreatedKvConsul]
        public async Task AddFileKv_with_urlFile_and_environment()
        {
            var configurationFiles = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                UrlFile = TestsConstants.PATH_FILE_OPTION1,
                RelativeRouteFileConsul = $"/{_prefix}"
            };

            bool result = await _consulKvService.AddFileKv(configurationFiles, "Development");
            result.Should().BeTrue();

            QueryResult<string[]> consulKeyOptions1 = await _client.KV.Keys($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1}");
            consulKeyOptions1.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{TestsConstants.PATH_FILE_OPTION1}"));

            QueryResult<string[]> consulKeyOptions1Development = await _client.KV.Keys($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1_DEVELOPMENT}");
            consulKeyOptions1Development.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                               .Should()
                                                                                               .Be(File.ReadAllText($"{_appPath}/{TestsConstants.PATH_FILE_OPTION1_DEVELOPMENT}"));
        }

        [Fact]
        [ResetCreatedKvConsul]
        public async Task AddFileKv_without_environment()
        {
            var configurationFiles = new ConsulConfigurationFile
            {
                Address = _client.Config.Address.OriginalString,
                UrlFile = TestsConstants.PATH_FILE_OPTION1,
                RelativeRouteFileConsul = $"/{_prefix}",
                UploadEnvironmentFile = false
            };

            bool result = await _consulKvService.AddFileKv(configurationFiles, null);
            result.Should().BeTrue();

            QueryResult<string[]> consulKeyOptions1 = await _client.KV.Keys($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1}");
            consulKeyOptions1.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{TestsConstants.PATH_FILE_OPTION1}"));
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

            QueryResult<string[]> consulKeyAppsettings = await _client.KV.Keys($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS}");
            consulKeyAppsettings.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS}").Result.Response.Value.BytesToString()
                                                                                       .Should()
                                                                                       .Be(File.ReadAllText($"{_appPath}/{TestsConstants.FILE_NAME_APPSETTINGS}"));

            QueryResult<string[]> consulKeyAppsettingsDevelopment = await _client.KV.Keys($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS_DEVELOPMENT}");
            consulKeyAppsettingsDevelopment.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                                   .Should()
                                                                                                   .Be(File.ReadAllText($"{_appPath}/{TestsConstants.FILE_NAME_APPSETTINGS_DEVELOPMENT}"));
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
                    TestsConstants.FILE_NAME_APPSETTINGS,
                    TestsConstants.PATH_FILE_OPTION1,
                    TestsConstants.PATH_FILE_OPTION2
                }
            };

            bool result = await _consulKvService.AddFileKv(configurationFiles, "Development");
            result.Should().BeTrue();

            QueryResult<string[]> consulKeyAppsettings = await _client.KV.Keys($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS}");
            consulKeyAppsettings.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS}").Result.Response.Value.BytesToString()
                                                                                       .Should()
                                                                                       .Be(File.ReadAllText($"{_appPath}/{TestsConstants.FILE_NAME_APPSETTINGS}"));

            QueryResult<string[]> consulKeyAppsettingsDevelopment = await _client.KV.Keys($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS_DEVELOPMENT}");
            consulKeyAppsettingsDevelopment.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.FILE_NAME_APPSETTINGS_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                                   .Should()
                                                                                                   .Be(File.ReadAllText($"{_appPath}/{TestsConstants.FILE_NAME_APPSETTINGS_DEVELOPMENT}"));

            QueryResult<string[]> consulKeyOptions1 = await _client.KV.Keys($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1}");
            consulKeyOptions1.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{TestsConstants.PATH_FILE_OPTION1}"));

            QueryResult<string[]> consulKeyOptions1Development = await _client.KV.Keys($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1_DEVELOPMENT}");
            consulKeyOptions1Development.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION1_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                               .Should()
                                                                                               .Be(File.ReadAllText($"{_appPath}/{TestsConstants.PATH_FILE_OPTION1_DEVELOPMENT}"));

            QueryResult<string[]> consulKeyOptions2 = await _client.KV.Keys($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION2}");
            consulKeyOptions2.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION2}").Result.Response.Value.BytesToString()
                                                                                   .Should()
                                                                                   .Be(File.ReadAllText($"{_appPath}/{TestsConstants.PATH_FILE_OPTION2}"));

            QueryResult<string[]> consulKeyOptions2Development = await _client.KV.Keys($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION2_DEVELOPMENT}");
            consulKeyOptions2Development.StatusCode.Should().Be(HttpStatusCode.OK);
            _client.KV.Get($"/{_prefix}/{TestsConstants.PATH_FILE_OPTION2_DEVELOPMENT}").Result.Response.Value.BytesToString()
                                                                                               .Should()
                                                                                               .Be(File.ReadAllText($"{_appPath}/{TestsConstants.PATH_FILE_OPTION2_DEVELOPMENT}"));


        }

        [Fact]
        public async Task AddFileKv_when_put_file_consul_return_badrequest()
        {
            Mock<IConsulClientService> mockconsulClientService = new Mock<IConsulClientService>();

            mockconsulClientService.Setup(x => x.UploadFile(It.IsAny<HttpClient>(), It.IsAny<ConsulConfigurationFile>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(async () =>
                                   {
                                       await Task.Delay(200);
                                       return new Response<string> { HttpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest }};
                                   });

            mockconsulClientService.As<IConsulClientService>().Setup(x => x.CreateConsulClient(It.IsAny<string>())).Returns(new HttpClient());
            mockconsulClientService.As<IConsulClientService>().Setup(x => x.GetListKv(It.IsAny<HttpClient>()))
                                                              .Returns(async () =>
                                                              {
                                                                  await Task.Delay(200);
                                                                  return new Response<List<string>>
                                                                  {
                                                                      Entity = new List<string> { "appsettings.json" }
                                                                  };
                                                              });

            _consulKvService = new ConsulKvService(mockconsulClientService.Object);

            bool result = await _consulKvService.AddFileKv(new ConsulConfigurationFile() { Address = _client.Config.Address.OriginalString }, "Development");
            result.Should().BeFalse();
        }

        #endregion

        #region Private Methods 

        private IConsulClientService CreateIConsulClientService()
        {
            Mock<IHttpClientFactory> consulclientFactory = new Mock<IHttpClientFactory>();

            consulclientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            return new ConsulClientService(consulclientFactory.Object);
        }

        private IConsulKvService CreateIConsulKvService()
            => new ConsulKvService(CreateIConsulClientService());

        #endregion
    }
}
