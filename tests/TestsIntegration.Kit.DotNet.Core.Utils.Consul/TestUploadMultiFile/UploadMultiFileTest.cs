using Consul;
using Tests.Tests.Infrastructure.GlobalConfiguration;
using Xunit;
using Kit.DotNet.Core.Utils.Extensions;
using FluentAssertions;
using System.Reflection;
using System.IO;
using TestsIntegration.Kit.DotNet.Core.Utils.Consul.TestUploadMultiFile.ServerConfiguration;

namespace TestsIntegration.Kit.DotNet.Core.Utils.Consul.TestUploadMultiFile
{
    [Collection(ServerFixture.SERVER_NAME)]
    public class UploadMultiFileTest
    {
        #region Properties

        private readonly ConsulClient _client;
        private readonly string _prefix;
        private readonly string _appPath;

        #endregion

        #region Constructors

        public UploadMultiFileTest()
        {
            _client = new ConsulClient();
            _prefix = ResetCreatedKvConsulAttribute.PREFIX_CONSUL_TEST;
            _appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        #endregion

        [Fact]
        [ResetCreatedKvConsul]
        public void UploadMultiFile_when_is_ok()
        {
            string contentFile = File.ReadAllText($"{_appPath}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_CONFIGURATION_FILE}.json");
            _client.KV.Get($"/{_prefix}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_CONFIGURATION_FILE}.json").Result.Response.Value.BytesToString()
                                                                                                                                  .Should()
                                                                                                                                  .Be(contentFile);

            string contentFileOptionsTest = File.ReadAllText($"{_appPath}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_OPTION_TESTS_FILE}.json");
            _client.KV.Get($"/{_prefix}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_OPTION_TESTS_FILE}.json").Result.Response.Value.BytesToString()
                                                                                                                                  .Should()
                                                                                                                                  .Be(contentFileOptionsTest);

            string contentFileOptionsTest2 = File.ReadAllText($"{_appPath}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_OPTION_TESTS_2_FILE}.json");
            _client.KV.Get($"/{_prefix}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_OPTION_TESTS_2_FILE}.json").Result.Response.Value.BytesToString()
                                                                                                                                  .Should()
                                                                                                                                  .Be(contentFileOptionsTest2);

            string contentFileOptionsTest3 = File.ReadAllText($"{_appPath}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_OPTION_TESTS_3_FILE}.json");
            _client.KV.Get($"/{_prefix}/{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}/{ServerFixture.NAME_OPTION_TESTS_3_FILE}.json").Result.Response.Value.BytesToString()
                                                                                                                                  .Should()
                                                                                                                                  .Be(contentFileOptionsTest3);
        }
    }
}
