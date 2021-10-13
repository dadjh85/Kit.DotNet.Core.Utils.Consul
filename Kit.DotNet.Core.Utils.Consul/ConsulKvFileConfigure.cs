using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Consul.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kit.DotNet.Core.Utils.Consul
{
    public static class ConsulKvFileConfigure
    {
        private const string CONSUL_CONFIG_SECTION = "ConsulConfig";

        public static IServiceCollection AddFilesKvConsul(this IServiceCollection services, IConfiguration configuration, string urlFile = null)
        {
            services.AddSingleton(LoadConsulKvOptions(configuration, urlFile));

            services.AddHostedService<ConsulKvUploadFilesHostedService>();
            services.AddTransient<IConsulKvService, ConsulKvService>();

            return services;
        }

        private static ConsulConfigurationFile LoadConsulKvOptions(IConfiguration configuration, string urlFile = null)
        {
            ConsulKvFileUserOptions consulKvFileUserOptions = new ConsulKvFileUserOptions();
            configuration.GetSection(CONSUL_CONFIG_SECTION).Bind(consulKvFileUserOptions);

            var consulConfigurationFile = new ConsulConfigurationFile { Address = consulKvFileUserOptions.Address };

            if (urlFile != null)
                consulConfigurationFile.UrlFile = urlFile;

            if (consulKvFileUserOptions.UrlFiles != null)
                consulConfigurationFile.UrlFiles = consulKvFileUserOptions.UrlFiles;

            if (consulKvFileUserOptions.UploadEnviromentFile != null)
                consulConfigurationFile.UploadEnvironmentFile = (bool)consulKvFileUserOptions.UploadEnviromentFile;

            if (consulKvFileUserOptions.ReloadConfigurationAllStartup != null)
                consulConfigurationFile.ReloadConfigurationAllStartup = (bool)consulKvFileUserOptions.ReloadConfigurationAllStartup;

            return consulConfigurationFile;
        }
    }
}
