using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using HumanaEdge.Webcore.Core.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="IHostBuilder" />.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the use of application specific dependency injection.
        /// </summary>
        /// <param name="hostBuilder">The web host builder.</param>
        /// <typeparam name="TEntry">A type located in the entry assembly.</typeparam>
        /// <returns>The same web host builder for fluent chaining.</returns>
        public static IHostBuilder UseDependencyInjection<TEntry>(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseServiceProviderFactory(
                new AutofacServiceProviderFactory(
                    cb => cb.RegisterWebcoreAttributedComponents<TEntry>()));
        }

        /// <summary>
        /// Performs dependency injection registration for all types decorated with <see cref="DiOptionsAttribute" />.
        /// </summary>
        /// <param name="services">The running service collection.</param>
        /// <param name="assemblies">The assemblies which should be scanned for injectable components.</param>
        /// <param name="configuration">The configuration settings of the app.</param>
        public static void UseOptionsPattern(
            this IServiceCollection services,
            Assembly[] assemblies,
            IConfiguration configuration)
        {
            services.AddOptionServices(configuration, assemblies);
        }
    }
}