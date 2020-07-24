using HumanaEdge.Webcore.Core.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.Encryption.Extensions
{
    /// <summary>
    /// Extension methods to add the required services for encryption to the <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Extension method for adding the ability to encrypt data using KMS.
        /// </summary>
        /// <param name="services">The service container for the application.</param>
        /// <param name="configuration">Configuration for the encryption service.</param>
        /// <returns>Service Collection with required config/services.</returns>
        public static IServiceCollection AddKmsEncryption(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.AddTransient<IKeyManagementServiceClientFactory, KeyManagementServiceClientFactory>();
            services.AddTransient<IEncryptionService, KmsEncryptionService>();
            services.Configure<EncryptionServiceOptions>(configuration);
            return services;
        }
    }
}