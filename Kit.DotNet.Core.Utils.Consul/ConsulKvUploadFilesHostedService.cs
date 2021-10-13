using Kit.DotNet.Core.Utils.Consul.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Kit.DotNet.Core.Utils.Consul.Models;

namespace Kit.DotNet.Core.Utils.Consul
{
    public class ConsulKvUploadFilesHostedService : IHostedService
    {
        private readonly ILogger<ConsulKvUploadFilesHostedService> _logger;
        private readonly ConsulConfigurationFile _consulConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ConsulKvUploadFilesHostedService(ILogger<ConsulKvUploadFilesHostedService> logger,  IServiceScopeFactory serviceScopeFactory, ConsulConfigurationFile consulConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _consulConfiguration = consulConfiguration ?? throw new ArgumentNullException(nameof(consulConfiguration));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _logger.LogInformation("uploading configuration files to consul");
                IConsulKvService consulKvService = scope.ServiceProvider.GetRequiredService<IConsulKvService>();

                string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                bool result = await consulKvService.AddFileKv(_consulConfiguration, environment);

                if(result)
                    _logger.LogInformation("the configuration files have been uploaded to consul");
                else
                {
                    _logger.LogError("there has been a problem uploading configuration files to consul");
                    throw new InvalidOperationException("a problem occurred while connecting to consul");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("the ConsulKvUploadFilesHostedService has been terminated.");
            //TODO: PENDING THE IMPLEMENTATION OF THE OPTION TO EXTRACT THE CONFIGURATION OF THE CONSUL
        }
    }
}
