using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Web.Resiliency;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                    r => r.StatusCode >= HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(
                    backOffIntervals,
                    onRetry: (outcome, duration, retryNumber, context) =>
                    {
                        var logger = context.GetLogger<IRestClient>();

                        logger.LogInformation(
                            "Http response status code {StatusCode}, retry attempt number {RetryNumber}. Retrying in {Duration}",
                            retryNumber,
                            outcome.Result.StatusCode,
                            duration);
                    });
        }

        /// <summary>
        /// Creates a resiliency policy with circuit breaker.
        /// </summary>
        /// <param name="durationOfBreakInTimeSpan">The number of seconds or minutes between breaks.</param>
        /// <param name="eventsAllowedBeforeBreaking">The number of events allowed before breaking.</param>
        /// <returns> The resilience policy. </returns>
        public static IAsyncPolicy<BaseRestResponse> CircuitBreaker(
            TimeSpan durationOfBreakInTimeSpan,
            int eventsAllowedBeforeBreaking) => Policy<BaseRestResponse>.HandleResult(
                r => r.StatusCode >= HttpStatusCode.InternalServerError)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: eventsAllowedBeforeBreaking,
                    durationOfBreak: durationOfBreakInTimeSpan,
                    onBreak: (result, state, duration, context) =>
                    {
                        var logger = context.GetLogger<IRestClient>();
                        logger.LogInformation("Circuit breaker state is now {State}", state.ToString());
                    },
                    onReset: (context) =>
                    {
                        var logger = context.GetLogger<IRestClient>();
                        logger.LogInformation("Circuit breaker has been reset and is in {State}.", "closed");
                    },
                    onHalfOpen: () =>
                    {
                    });

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