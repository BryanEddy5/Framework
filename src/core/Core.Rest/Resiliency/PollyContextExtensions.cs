using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using Microsoft.Extensions.Logging;
using Polly;

namespace HumanaEdge.Webcore.Core.Rest.Resiliency
{
    /// <summary>
    /// Extension methods for polly <see cref="Context"/> exclusive to REST.
    /// </summary>
    public static class PollyContextExtensions
    {
        private static readonly string RestRequestKey = "OriginalRestRequest";

        private static readonly string AccessTokenStorageKey = "AccessTokenStorageKey";

        /// <summary>
        /// Adds the RestRequest to the polly context.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <param name="restRequest">The rest request to be stored.</param>
        /// <returns>The same context for fluent chaining.</returns>
        public static Context WithRestRequest(this Context context, RestRequest restRequest)
        {
            context[RestRequestKey] = restRequest;
            return context;
        }

        /// <summary>
        /// Retrieves the RestRequest.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <returns>The original rest request.</returns>
        public static RestRequest GetRestRequest(this Context context)
        {
            if (context.TryGetValue(RestRequestKey, out object restRequest))
            {
                return (restRequest as RestRequest)!;
            }

            return null!;
        }

        /// <summary>
        /// Adds the <see cref="IAccessTokenCacheService"/> to the polly context.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <param name="accessTokenCacheService">The access token cache service.</param>
        /// <returns>The same context for fluent chaining.</returns>
        public static Context WithAccessTokenCache(this Context context, IAccessTokenCacheService accessTokenCacheService)
        {
            context[AccessTokenStorageKey] = accessTokenCacheService;
            return context;
        }

        /// <summary>
        /// Retrieves the <see cref="IAccessTokenCacheService"/>.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <returns>The access token cache.</returns>
        public static IAccessTokenCacheService GetAccessTokenCache(this Context context)
        {
            if (context.TryGetValue(AccessTokenStorageKey, out object accessTokenCache))
            {
                return (accessTokenCache as IAccessTokenCacheService)!;
            }

            return null!;
        }
    }
}