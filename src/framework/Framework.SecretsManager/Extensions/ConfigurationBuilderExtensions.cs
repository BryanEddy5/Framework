using System;
using Google.Api;
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
        /// The environment variable for the project that contains the secret.
        /// </summary>
        private const string SecretProjectEnvironmentVariable = "GCP_SECRETS_PROJECT";

        /// <summary>
        /// The environment variable for the secret name.
        /// </summary>
        private const string SecretNameEnvironmentVariable = "GITLAB_PROJECT_NAME";

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
            var secretProject = Environment.GetEnvironmentVariable(SecretProjectEnvironmentVariable);
            secretId ??= Environment.GetEnvironmentVariable(SecretNameEnvironmentVariable);
            var isActive = Environment.GetEnvironmentVariable("GCP_SECRETS_ACTIVE");
            if (isActive == "true")
            {
                if (secretProject == null)
                {
                    throw new ArgumentNullException($"The environment variable {SecretProjectEnvironmentVariable} is null");
                }

                if (secretId == null)
                {
                    throw new ArgumentNullException($"The environment variable {SecretNameEnvironmentVariable} is null");
                }

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