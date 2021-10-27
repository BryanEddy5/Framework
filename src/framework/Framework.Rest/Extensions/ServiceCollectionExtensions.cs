using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Framework.Rest.AccessTokens;
using HumanaEdge.Webcore.Framework.Rest.Alerting;
using HumanaEdge.Webcore.Framework.Rest.Resiliency;
using HumanaEdge.Webcore.Framework.Rest.Transformations;
using Microsoft.AspNetCore.Http;
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
            services.AddTransient<IHttpAlertingService, HttpAlertingService>();
            services.AddSingleton<IRestClientFactory, RestClientFactory>();
            services.AddSingleton<IAccessTokenCacheService, AccessTokenCacheService>();
            services.AddSingleton<IPollyContextFactory, PollyContextFactory>();
            services.AddSingleton<IRequestTransformationFactory, RequestTransformationFactory>();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMediaTypeFormatter, JsonMediaTypeFormatter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMediaTypeFormatter, XmlMediaTypeFormatter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMediaTypeFormatter, FormUrlEncodedMediaTypeFormatter>());
            return services;
        }
    }
}