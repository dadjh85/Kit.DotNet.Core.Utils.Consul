using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Kit.DotNet.Core.Utils.Consul;
using Microsoft.AspNetCore.Builder;
using Tests.Tests.Infrastructure.GlobalConfiguration;

namespace TestsIntegration.Kit.DotNet.Core.Utils.Consul.TestUploadMultiFile.ServerConfiguration
{
    public class TestStartup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public TestStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddFilesKvConsul(configuration: Configuration, 
                                      relativeRouteFileConsul: ResetCreatedKvConsulAttribute.PREFIX_CONSUL_TEST);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
