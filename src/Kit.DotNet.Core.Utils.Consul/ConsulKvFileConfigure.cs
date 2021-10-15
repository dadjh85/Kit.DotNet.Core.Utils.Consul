using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Consul.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kit.DotNet.Core.Utils.Consul
{
    /// <summary>
    /// class for configurate the upload of the files in consul server 
    /// </summary>
    public static class ConsulKvFileConfigure
    {
        /// <summary>
        /// constant with the library configuration in the appsetting.json of the user's net core application
        /// </summary>
        private const string CONSUL_CONFIG_SECTION = "ConsulConfig";

        /// <summary>
        /// configuration method for registering the process in the user's .NET Core application
        /// </summary>
        /// <param name="services">a object IServiceCollection for register the configuration in the method of ConfigureService of net core aplication</param>
        /// <param name="configuration">a object IConfiguration whith the user's options</param>
        /// <param name="urlFile">the url of file to upload in consul</param>
        /// <param name="relativeRouteFileConsul">relative path of the file in consul in case you want to upload the files inside a folder</param>
        /// <returns>a object IServiceCollection</returns>
        public static IServiceCollection AddFilesKvConsul(this IServiceCollection services, IConfiguration configuration, string urlFile = null, string relativeRouteFileConsul = null)
        {
            services.AddSingleton(LoadConsulKvOptions(configuration, urlFile, relativeRouteFileConsul));

            services.AddHostedService<ConsulKvUploadFilesHostedService>();
            services.AddTransient<IConsulKvService, ConsulKvService>();

            return services;
        }

        /// <summary>
        /// method for process the user configuration
        /// </summary>
        /// <param name="configuration">a object IConfiguration whith the user's options</param>
        /// <param name="urlFile">the url of file to upload in consul</param>
        /// <param name="relativeRouteFileConsul">relative path of the file in consul in case you want to upload the files inside a folder</param>
        /// <returns>a object ConsulConfigurationFile with the user options</returns>
        private static ConsulConfigurationFile LoadConsulKvOptions(IConfiguration configuration, string urlFile, string relativeRouteFileConsul)
        {
            ConsulKvFileUserOptions consulKvFileUserOptions = new ConsulKvFileUserOptions();
            configuration.GetSection(CONSUL_CONFIG_SECTION).Bind(consulKvFileUserOptions);

            var consulConfigurationFile = new ConsulConfigurationFile { Address = consulKvFileUserOptions.Address };

            if (urlFile != null)
                consulConfigurationFile.UrlFile = urlFile;

            if (relativeRouteFileConsul != null)
                consulConfigurationFile.RelativeRouteFileConsul = $"{relativeRouteFileConsul}/";

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
