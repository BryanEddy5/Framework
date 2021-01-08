using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HumanaEdge.Webcore.Framework.Testing.EnvironmentSetup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HumanaEdge.Webcore.Framework.Testing.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IHostBuilder" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// An extension method to bootstrap the <see cref="IHostBuilder" /> for integration testing settings.
        /// </summary>
        /// <param name="hostBuilder"> The host builder. </param>
        /// <param name="configureDelegate"> A delegate for configuration services. </param>
        /// <returns> The same host builder for fluent chaining. </returns>
        public static IHostBuilder UseCustomTestConfiguration(
            this IHostBuilder hostBuilder,
            Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            return hostBuilder.UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureServices(configureDelegate);
        }

        private static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder config)
        {
            var env = TestEnvironmentHandler.GetEnvironment;
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Configuration.{env}.json", false, true)
                .AddJsonFile("Secrets/Configuration.Secrets.json", true, true)
                .AddEnvironmentVariables($"TEST_SECRET_{env}");
        }
    }
}