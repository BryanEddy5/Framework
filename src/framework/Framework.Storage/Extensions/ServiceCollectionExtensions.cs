using HumanaEdge.Webcore.Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.Storage.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection" /> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add pub sub services to the application.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddStorageClient(this IServiceCollection services)
        {
            services.AddSingleton<IStorageClient, GcpStorageClient>();
        }
    }
}