using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Kit.DotNet.Core.Utils.Consul;
using Microsoft.AspNetCore.Builder;
using TestsIntegration.Kit.DotNet.Core.Utils.Consul.ServerConfiguration;
using Tests.Tests.Infrastructure.GlobalConfiguration;

namespace TestsIntegration.Kit.DotNet.Core.Utils.Consul
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
                                      urlFile: $"{ServerFixture.RELATIVE_PATH_OPTIONS_FILES}\\appsettings.json", 
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
