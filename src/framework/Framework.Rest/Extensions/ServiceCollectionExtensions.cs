using HumanaEdge.Webcore.Core.Rest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HumanaEdge.Webcore.Framework.Rest.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services necessary for rest client support.
        /// </summary>
        /// <param name="services">The running service collection.</param>
        /// <returns>The service collection, for fluent chaining.</returns>
        public static IServiceCollection AddRestClient(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IInternalClientFactory, InternalClientFactory>();
            services.AddSingleton<IRestClientFactory, RestClientFactory>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMediaTypeFormatter, JsonMediaTypeFormatter>());
            return services;
        }
    }
}