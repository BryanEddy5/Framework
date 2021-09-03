using System;
using HumanaEdge.Webcore.Core.Caching.Options;
using HumanaEdge.Webcore.Framework.Caching.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace HumanaEdge.Webcore.Core.Caching.Extensions
{
    /// <summary>
    /// Extension methods for registering the caching library.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// The configuration key for the current hosting environment.
        /// </summary>
        internal const string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";

        /// <summary>
        /// The identifier for the development hosting environment.
        /// </summary>
        internal const string DevelopmentEnvironment = "development";

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
            var env = Environment.GetEnvironmentVariable(EnvironmentKey) ?? string.Empty;
            if (env.Equals(DevelopmentEnvironment, StringComparison.OrdinalIgnoreCase))
            {
                return services.AddDistributedMemoryCache();
            }

            var certificateValidator =
                services.BuildServiceProvider().GetService<ICertificateValidationFactory>()!.Create();

            return services.AddStackExchangeRedisCache(
                options =>
                {
                    options.ConfigurationOptions = ConfigurationOptions.Parse(configurationSection["ConnectionString"]);
                    options.ConfigurationOptions.CertificateValidation += certificateValidator;
                });
        }
    }
}