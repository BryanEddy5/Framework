using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Web.Resiliency;
using Microsoft.Extensions.Logging;
using Polly;

namespace HumanaEdge.Webcore.Core.Soap.Resilience
{
    /// <summary>
    /// A class with pre-created resiliency policies designed for use w/ <see cref="BaseSoapClient{TClient,TChannel}"/>.
    /// </summary>
    public static class ResiliencyPolicies
    {
        /// <summary>
        /// Creates a resiliency policy with exponential backoff.
        /// </summary>
        /// <param name="maxRetryAttempts">The maximum number of retries.</param>
        /// <returns> The resilience policy. </returns>
        public static IAsyncPolicy<HttpResponseMessage> RetryWithExponentialBackoff(int maxRetryAttempts)
        {
            var jitterer = new Random();
            var backOffIntervals = Enumerable.Range(1, maxRetryAttempts)
                .Select(
                    t => TimeSpan.FromMilliseconds(Math.Pow(2, t)) +
                         TimeSpan.FromMilliseconds(jitterer.Next(0, 100)))
                .ToArray();
            var policy = Policy<HttpResponseMessage>
                .HandleResult(r => r.StatusCode >= HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(
                    backOffIntervals,
                    (outcome, duration, retryNumber, context) =>
                    {
                        var logger = context.GetLogger("SoapClient");
                        logger.LogInformation(
                            "Http response status code {StatusCode}, retry#{RetryNumber}. Retrying in {Duration}",
                            outcome.Result.StatusCode,
                            retryNumber,
                            duration);
                    });

            return policy;
        }

        /// <summary>
        /// Creates a resiliency policy with circuit breaker.
        /// </summary>
        /// <param name="durationOfBreakInTimeSpan">The number of seconds or minutes between breaks.</param>
        /// <param name="eventsAllowedBeforeBreaking">The number of events allowed before breaking.</param>
        /// <returns> The resilience policy. </returns>
        public static IAsyncPolicy<HttpResponseMessage> CircuitBreaker(
            TimeSpan durationOfBreakInTimeSpan,
            int eventsAllowedBeforeBreaking) => Policy<HttpResponseMessage>.HandleResult(
                r => r.StatusCode >= HttpStatusCode.InternalServerError)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: eventsAllowedBeforeBreaking,
                durationOfBreak: durationOfBreakInTimeSpan,
                onBreak: (result, state, duration, context) =>
                {
                    var logger = context.GetLogger("SoapClient");
                    logger.LogInformation("Circuit breaker state is now {State}", state.ToString());
                },
                onReset: (context) =>
                {
                    var logger = context.GetLogger("SoapClient");
                    logger.LogInformation("Circuit breaker has been reset and is in {State}.", "closed");
                },
                onHalfOpen: () =>
                {
                });
    }
}