using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Polly;

namespace HumanaEdge.Webcore.Core.Rest.Resiliency
{
    /// <summary>
    /// A class with pre-created resiliency policies.
    /// </summary>
    public static class ResiliencyPolicies
    {
        /// <summary>
        /// Creates a resiliency policy with exponential backoff.
        /// </summary>
        /// <param name="maxRetryAttempts">The maximum number of retries.</param>
        /// <returns> The resilience policy. </returns>
        public static IAsyncPolicy<BaseRestResponse> RetryWithExponentialBackoff(int maxRetryAttempts)
        {
            var jitterer = new Random();
            var backOffIntervals = Enumerable.Range(1, maxRetryAttempts)
                .Select(
                    t => TimeSpan.FromMilliseconds(Math.Pow(2, t)) +
                         TimeSpan.FromMilliseconds(jitterer.Next(0, 100)))
                .ToArray();
            return Policy<BaseRestResponse>.HandleResult(
                    r => (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(backOffIntervals);
        }

        /// <summary>
        /// Creates a resiliency policy for refreshing a token on a 401 response.
        /// </summary>
        /// <param name="tokenFactory">A factory for creating the token.</param>
        /// <param name="tokenClientKey">The unique identifier for fetching and storing the cached token.</param>
        /// <typeparam name="TClient">The client type for adding soruce context to logging.</typeparam>
        /// <returns>The created policy.</returns>
        public static IAsyncPolicy<BaseRestResponse> RefreshToken<TClient>(
            Func<CancellationToken, Task<string>> tokenFactory,
            string tokenClientKey) =>
            Policy<BaseRestResponse>
                .HandleResult(r => r.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(
                    1,
                    async (delegateResult, retryNumber, context) =>
                    {
                        var accessTokenCache = context.GetAccessTokenCache();
                        var logger = context.GetLogger<TClient>();

                        logger.LogInformation("401 received; retrieving new bearer token...");
                        var newBearerToken = await accessTokenCache.GetAsync(
                            tokenFactory,
                            tokenClientKey,
                            CancellationToken.None,
                            true);

                        var currentRequest = context.GetRestRequest();
                        currentRequest!.Headers[HeaderNames.Authorization] = $"Bearer {newBearerToken}";
                    });
    }
}