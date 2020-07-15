using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Framework.Logging.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HumanaEdge.Webcore.Framework.Logging.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="IWebHostBuilder" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the use of application specific logging.
        /// </summary>
        /// <param name="hostBuilder">The web host builder.</param>
        /// <typeparam name="TEntry">A type from the entry point assembly.</typeparam>
        /// <returns>The same web host builder for fluent chaining.</returns>
        public static IHostBuilder UseAppLogging<TEntry>(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureLogging(
                    (context, logging) =>
                    {
                        logging.ClearProviders();
                        LoggingAppConfiguration.Configure<TEntry>(context.Configuration);
                    })
                .UseSerilog();
        }
    }
}