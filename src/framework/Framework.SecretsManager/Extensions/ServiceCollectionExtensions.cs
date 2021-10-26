using System;
using HumanaEdge.Webcore.Core.SecretsManager;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using HumanaEdge.Webcore.Framework.SecretsManager.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="ISecretsClient"/> for dependency injection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection for fluent chaining.</returns>
        public static IServiceCollection AddSecretsClient(
            this IServiceCollection services)
        {
            return services.AddSingleton<ISecretsClient, SecretsClient>();
        }

        /// <summary>
        /// Adds a secrets service for retrieving a particular secret from Secrets Manager.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="options">The configuration settings for retrieving the secret.</param>
        /// <typeparam name="TSecret">The secret's shape.</typeparam>
        /// <returns>The same service collection for fluent chaining.</returns>
        public static IServiceCollection AddSecret<TSecret>(
            this IServiceCollection services,
            Action<SecretsOptions> options)
            where TSecret : ISecret
        {
            services.Configure(options);
            return services.AddSecret<TSecret>();
        }

        /// <summary>
        /// Adds a secrets service for retrieving a particular secret from Secrets Manager.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Configuration for the encryption service.</param>
        /// <typeparam name="TSecret">The secret's shape.</typeparam>
        /// <typeparam name="TSecretOptions">The secrets options object.</typeparam>
        /// <returns>The same service collection for fluent chaining.</returns>
        public static IServiceCollection AddSecret<TSecret, TSecretOptions>(
            this IServiceCollection services,
            IConfigurationSection configuration)
            where TSecret : ISecret
            where TSecretOptions : SecretsOptions
        {
            services.Configure<SecretsOptions>(typeof(TSecret).FullName, configuration);
            return services.AddSecret<TSecret>();
        }

        /// <summary>
        /// Adds a secrets service for retrieving a particular secret from Secrets Manager.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Configuration for the encryption service.</param>
        /// <typeparam name="TSecret">The secret's shape.</typeparam>
        /// <returns>The same service collection for fluent chaining.</returns>
        public static IServiceCollection AddSecret<TSecret>(
            this IServiceCollection services,
            IConfigurationSection configuration)
            where TSecret : ISecret
        {
            services.Configure<SecretsOptions>(typeof(TSecret).FullName, configuration);
            return services.AddSecret<TSecret>();
        }

        private static IServiceCollection AddSecret<TSecret>(this IServiceCollection services)
            where TSecret : ISecret
        {
            services.AddSingleton<ISecretsService<TSecret>, SecretsService<TSecret>>();
            services.AddSingleton<ISecretsHandler, SecretsHandler>();
            services.AddMemoryCache();
            return services.AddSingleton<ISecretsClient, SecretsClient>();
        }
    }
}