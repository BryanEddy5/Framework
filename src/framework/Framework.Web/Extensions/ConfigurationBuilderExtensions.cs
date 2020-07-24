using System;
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
        /// <returns>fluently returns another <see cref="IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddConfigOptions(this IConfigurationBuilder builder)
        {
            builder.AddJsonFile(
                $"{Environment.GetEnvironmentVariable("APP_ROOT")}/config/appsettings.overrides.json",
                optional: true,
                reloadOnChange: true);

            var secretsFilePathFromEnvironment = Environment.GetEnvironmentVariable("SECRETS_PATH");
            var secretsFilePath = string.IsNullOrWhiteSpace(secretsFilePathFromEnvironment)
                ? "secrets/appsettings.Secrets.json"
                : secretsFilePathFromEnvironment;

            // values override in order added
            return builder.AddJsonFile(secretsFilePath, true, true);
        }
    }
}