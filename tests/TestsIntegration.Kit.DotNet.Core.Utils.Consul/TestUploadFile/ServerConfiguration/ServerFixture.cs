﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace TestsIntegration.Kit.DotNet.Core.Utils.Consul.ServerConfiguration
{
    public class ServerFixture
    {
        #region Properties

        public TestServer TestServer { get; set; }

        #endregion

        #region constants

        public const string ENVIRONMENT = "Development";
        public const string RELATIVE_PATH_OPTIONS_FILES = "TestUploadFile\\Optionsfiles";
        public const string SERVER_NAME = "ServerFixture";
        public const string NAME_CONFIGURATION_FILE = "appsettings";

        #endregion

        #region Constructor

        public ServerFixture()
        {
            TestServer = new TestServer(ConfigureHostBuilder());
        }

        #endregion

        public IWebHostBuilder ConfigureHostBuilder()
        {
            return new WebHostBuilder().ConfigureAppConfiguration((builder, config) =>
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", ENVIRONMENT);

                string publishPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\{RELATIVE_PATH_OPTIONS_FILES}";

                config.SetBasePath(publishPath)
                      .AddJsonFile($"{NAME_CONFIGURATION_FILE}.json")
                      .AddJsonFile($"{NAME_CONFIGURATION_FILE}.{builder.HostingEnvironment?.EnvironmentName}.json");

                config.Build();
            })
            .UseEnvironment(ENVIRONMENT)
            .UseStartup<TestStartup>();
        }
    }

    [CollectionDefinition(ServerFixture.SERVER_NAME)]
    public class ServerFixtureCollection : ICollectionFixture<ServerFixture>
    {
    }
}
