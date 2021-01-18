using System;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using HumanaEdge.Webcore.Framework.SecretsManager.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

#pragma warning disable VSTHRD002
namespace HumanaEdge.Webcore.Framework.SecretsManager.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds secrets from GCP secrets manager to the configuration.
        /// </summary>
        /// <param name="builder">The config builder.</param>
        /// <param name="secretId">The unique identifier for the secret.</param>
        /// <returns>The same configuration builder for fluent chaining.</returns>
        public static IConfigurationBuilder AddGcpSecrets(
            this IConfigurationBuilder builder,
            string? secretId = null)
        {
            var secretProject = Environment.GetEnvironmentVariable("GCP_SECRETS_PROJECT");
            secretId ??= Environment.GetEnvironmentVariable("GITLAB_PROJECT_NAME");
            if (secretProject != null)
            {
                var stream = new InternalSecretsClient().GetAsync(
                        new SecretsOptions
                        {
                            ProjectId = secretProject,
                            SecretId = $"{secretId}_secret_json",
                            SecretVersionId = "latest"
                        },
                        default)
                    .GetAwaiter()
                    .GetResult();
                builder.Add<JsonStreamConfigurationSource>(s => s.Stream = stream);
            }

            return builder;
        }
    }
}

#pragma warning restore VSTHRD002