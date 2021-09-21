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
        /// <typeparam name="TClient">The type of soap client.</typeparam>
        /// <typeparam name="TChannel">The service contract associated with the soap client.</typeparam>
        /// <returns> The resilience policy. </returns>
        public static IAsyncPolicy<HttpResponseMessage> RetryWithExponentialBackoff<TClient, TChannel>(
            int maxRetryAttempts)
                where TClient : BaseSoapClient<TClient, TChannel>
                where TChannel : class
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
                        var logger = context.GetLogger<BaseSoapClient<TClient, TChannel>>();
                        logger.LogInformation(
                            "Http response status code {StatusCode}, retry#{RetryNumber}. Retrying in {Duration}",
                            retryNumber,
                            outcome.Result.StatusCode,
                            duration);
                    });

            return policy;
        }
    }
}