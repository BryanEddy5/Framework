using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Core.Caching.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IDistributedCache" />.
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// Retrieves the value associated with a key.
        /// </summary>
        /// <param name="cache"> The cache. </param>
        /// <param name="key"> The cache key. </param>
        /// <param name="factory"> A factory for creating the entry. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <typeparam name="T"> The type to have it deserialized to. </typeparam>
        /// <returns> The designated type. </returns>
        public static async Task<T> GetOrCreateAsync<T>(
            this IDistributedCache cache,
            string key,
            Func<CancellationToken, Task<T>> factory,
            CancellationToken cancellationToken)
        {
            return await cache.GetOrCreateAsync(key, factory, cancellationToken, new DistributedCacheEntryOptions());
        }

        /// <summary>
        /// Retrieves the value associated with a key.
        /// </summary>
        /// <param name="cache"> The cache. </param>
        /// <param name="key"> The cache key. </param>
        /// <param name="factory"> A factory for creating the entry. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <param name="cacheEntryOptions"> Configuration settings for the cache entry. </param>
        /// <typeparam name="T"> The type to have it deserialized to. </typeparam>
        /// <returns> The designated type. </returns>
        public static async Task<T> GetOrCreateAsync<T>(
            this IDistributedCache cache,
            string key,
            Func<CancellationToken, Task<T>> factory,
            CancellationToken cancellationToken,
            DistributedCacheEntryOptions cacheEntryOptions)
        {
            var result = await cache.GetStringAsync(key, cancellationToken);
            if (result == null)
            {
                var response = await factory.Invoke(cancellationToken);
                await cache.SetAsync(key, response!, cancellationToken, cacheEntryOptions);
                return response;
            }

            return JsonConvert.DeserializeObject<T>(result!);
        }

        /// <summary>
        /// Sets the value in the cache.
        /// </summary>
        /// <param name="cache"> The cache. </param>
        /// <param name="key"> The cache key. </param>
        /// <param name="value"> The value to be stored. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> An awaitable task. </returns>
        public static async Task SetAsync(
            this IDistributedCache cache,
            string key,
            object value,
            CancellationToken cancellationToken)
        {
            await cache.SetAsync(key, value, cancellationToken, new DistributedCacheEntryOptions());
        }

        /// <summary>
        /// Sets the value in the cache.
        /// </summary>
        /// <param name="cache"> The cache. </param>
        /// <param name="key"> The cache key. </param>
        /// <param name="value"> The value to be stored. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <param name="cacheEntryOptions"> Configuration settings for the cache entry. </param>
        /// <returns> An awaitable task. </returns>
        public static async Task SetAsync(
            this IDistributedCache cache,
            string key,
            object value,
            CancellationToken cancellationToken,
            DistributedCacheEntryOptions cacheEntryOptions)
        {
            var json = JsonConvert.SerializeObject(value);
            await cache.SetStringAsync(key, json, cacheEntryOptions, cancellationToken);
        }
    }
}