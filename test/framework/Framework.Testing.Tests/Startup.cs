using HumanaEdge.Webcore.Framework.Testing.Extensions;
using HumanaEdge.Webcore.Framework.Web.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HumanaEdge.Webcore.Framework.Testing.Tests
{
    /// <summary>
    /// Test startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures the host builder.
        /// </summary>
        /// <param name="hostBuilder">Creates the host.</param>
        public void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder
                .UseCustomHostBuilder<Startup>()
                .UseCustomTestConfiguration(ConfigureServices);
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var configuration = context.Configuration;
            services.AddTestClientFactory("Foo", configuration);
        }
    }
}