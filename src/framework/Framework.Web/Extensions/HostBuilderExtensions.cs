using HumanaEdge.Webcore.Framework.DependencyInjection.Extensions;
using HumanaEdge.Webcore.Framework.Logging.Extensions;
using Microsoft.Extensions.Hosting;

namespace HumanaEdge.Webcore.Framework.Web.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures custom configuration for <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="hostBuilder">The web host builder.</param>
        /// <typeparam name="TEntry">A type from the entry point assembly.</typeparam>
        /// <returns>The same web host builder for fluent chaining.</returns>
        public static IHostBuilder UseCustomHostBuilder<TEntry>(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseAppLogging<TEntry>()
                .UseDependencyInjection<TEntry>();
        }
    }
}