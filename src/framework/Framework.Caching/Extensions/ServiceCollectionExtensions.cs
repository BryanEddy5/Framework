using System;
using System.Security.Cryptography.X509Certificates;
using HumanaEdge.Webcore.Core.Caching.Options;
using HumanaEdge.Webcore.Framework.Caching.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace HumanaEdge.Webcore.Framework.Caching.Extensions
{
    /// <summary>
    /// Extension methods for registering the caching library.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// The configuration key for the current hosting environment.
        /// </summary>
        internal const string EnvironmentKey = "USE_IN_MEMORY_CACHE";

        /// <summary>
        /// Registers the services for distributed caching.
        /// </summary>
        /// <param name="services">The running service collection.</param>
        /// <param name="configurationSection">The section of the configuration file with the caching settings. </param>
        /// <returns>The same service collection for fluent chaining.</returns>
        public static IServiceCollection AddDistributedCache(
            this IServiceCollection services,
            IConfigurationSection configurationSection)
        {
            services.AddSingleton<ICertificateAuthorityService, CertificateAuthorityService>();
            services.AddSingleton<ICertificateValidationFactory, CertificateValidationFactory>();
            services.AddOptions<CacheOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations();
            var useInMemoryCache = Environment.GetEnvironmentVariable(EnvironmentKey);
            if (useInMemoryCache != null)
            {
                return services.AddDistributedMemoryCache();
            }

            var serviceProvider = services.BuildServiceProvider();
            var certificateValidator =
                serviceProvider.GetService<ICertificateValidationFactory>()!.Create();

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { configurationSection["ConnectionString"] },
                Ssl = true,
            };
            configurationOptions.CertificateValidation += certificateValidator;
            configurationOptions.CertificateSelection += (
                sender,
                targetHost,
                localCertificates,
                remoteCertificate,
                acceptableIssuers) =>
            {
                var certBytes = serviceProvider.GetService<ICertificateAuthorityService>()!.GetCertificate();
                var cert = new X509Certificate2(certBytes, string.Empty);
                return cert;
            };

            return services.AddStackExchangeRedisCache(
                options =>
                {
                    options.ConfigurationOptions = configurationOptions;
                });
        }
    }
}