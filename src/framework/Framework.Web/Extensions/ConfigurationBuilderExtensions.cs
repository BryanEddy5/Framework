using System;
using System.Linq;
using HumanaEdge.Webcore.Framework.SecretsManager.Extensions;
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
            string? secretsName = null)
        {
            var secretsFilePathFromEnvironment = Environment.GetEnvironmentVariable(secretsName ?? "SECRETS");
            var secretsFilePath = string.IsNullOrWhiteSpace(secretsFilePathFromEnvironment)
                ? "secrets/appsettings.Secrets.json"
                : secretsFilePathFromEnvironment;

            var reloadOnChange = false;

            // We don't hot swap configuration settings when deployed.  Hot reload configuration changes only when running locally.
#if DEBUG
            reloadOnChange = true;
#endif

            // values override in order added
            builder
                .AddJsonFile("appsettings.Local.json", true, reloadOnChange)
                .AddJsonFile(
                    $"{Environment.GetEnvironmentVariable("APP_ROOT")}/config/appsettings.overrides.json",
                    true,
                    reloadOnChange)
                .AddJsonFile(secretsFilePath, true, reloadOnChange)
                .AddGcpSecrets(secretsName)
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