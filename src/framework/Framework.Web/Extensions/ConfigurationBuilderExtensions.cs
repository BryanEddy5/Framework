using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace HumanaEdge.Webcore.Framework.Web.Extensions
{
    /// <summary>
    /// Adds extension methods to <see cref="IConfigurationBuilder" />.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Extension method for <see cref="IConfigurationBuilder" />.
        /// </summary>
        /// <param name="builder"><see cref="IConfigurationBuilder" />.</param>
        /// <param name="args">CLI arguments.</param>
        /// <param name="secretsName">The name of the environment variable associated with the secrets file.</param>
        /// <returns>fluently returns another <see cref="IConfigurationBuilder" />.</returns>
#pragma warning disable SA1011
        public static IConfigurationBuilder AddConfigOptions(
            this IConfigurationBuilder builder,
            string[]? args = null,
            string secretsName = "SECRETS")
        {
            var secretsFilePathFromEnvironment = Environment.GetEnvironmentVariable(secretsName);
            var secretsFilePath = string.IsNullOrWhiteSpace(secretsFilePathFromEnvironment)
                ? "secrets/appsettings.Secrets.json"
                : secretsFilePathFromEnvironment;

            // values override in order added
            builder
                .AddJsonFile("appsettings.Local.json", true, true)
                .AddJsonFile(
                    $"{Environment.GetEnvironmentVariable("APP_ROOT")}/config/appsettings.overrides.json",
                    true,
                    true)
                .AddJsonFile(secretsFilePath, true, true)
                .AddEnvironmentVariables();

            if (args?.Any() == true)
            {
                builder.AddCommandLine(args);
            }

            return builder;
        }
    }
}
#pragma warning restore SA1011