using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using Microsoft.Extensions.Logging;
using Polly;

namespace HumanaEdge.Webcore.Core.Rest.Resiliency
{
    /// <summary>
    /// Extension methods for polly <see cref="Context"/>.
    /// </summary>
    public static class PollyContextExtensions
    {
        private static readonly string LoggerKey = "LoggerKey";

        private static readonly string RestRequestKey = "OriginalRestRequest";

        private static readonly string AccessTokenStorageKey = "AccessTokenStorageKey";

        /// <summary>
        /// Sets a logger factory allowing consumers to access a logger without injecting it.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <param name="loggerFactory">A logger factory for generating a logger instance.</param>
        /// <returns>The same context for fluent chaining.</returns>
        public static Context WithLogger(this Context context, ILoggerFactory loggerFactory)
        {
            context[LoggerKey] = loggerFactory;
            return context;
        }

        /// <summary>
        /// Retrieves a logger factory allowing consumers to access a logger without injecting it.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <typeparam name="T">The source context for logging.</typeparam>
        /// <returns>A logger.</returns>
        public static ILogger<T> GetLogger<T>(this Context context)
        {
            if (context.TryGetValue(LoggerKey, out object loggerFactory))
            {
                var typeLoggerFactory = loggerFactory as ILoggerFactory;
                return typeLoggerFactory.CreateLogger<T>();
            }

            return null!;
        }

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