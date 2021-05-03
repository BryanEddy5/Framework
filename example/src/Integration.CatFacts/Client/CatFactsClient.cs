using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Core.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Client
{
    /// <inheritdoc />
    [DependencyInjectedComponent]
    internal sealed class CatFactsClient : BaseRestClient<CatFactsClient>, ICatFactsClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatFactsClient"/> class.
        /// </summary>
        /// <param name="restClientFactory">A factory pattern for generating the http client.</param>
        /// <param name="options">Client configuration options.</param>
        /// <param name="logger">The application logger.</param>
        public CatFactsClient(
            IRestClientFactory restClientFactory,
            IOptionsSnapshot<CatFactsClientOptions> options,
            ILogger<CatFactsClient> logger)
            : base(restClientFactory, ConfigureRestOptions(options.Value, logger))
        {
        }

        private static RestClientOptions ConfigureRestOptions(
            CatFactsClientOptions options,
            ILogger<CatFactsClient> logger)
        {
            var jitterer = new Random();
            var timeout = TimeSpan.FromMilliseconds(options.TimeoutMilliseconds);
            var maxAttempts = options.Resilience.RetryAttempts;
            var backOffIntervals = Enumerable.Range(1, maxAttempts)
                .Select(
                    t => TimeSpan.FromMilliseconds(Math.Pow(2, t)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)))
                .ToArray();

            // create the appropriate resiliency policy.
            var resiliencePolicy = Policy<BaseRestResponse>.HandleResult(
                    r => r.StatusCode == HttpStatusCode.BadGateway || r.StatusCode == HttpStatusCode.GatewayTimeout)
                .WaitAndRetryAsync(
                    backOffIntervals,
                    (result, backOffInterval, totalAttempts, context) =>
                    {
                        logger.LogWarning(
                            new EventId(1),
                            "REST call to Cat Facts Api failed, will be retried. Total attempts: {TotalAttempts} of {MaxAttempts}; HTTP Status: {HttpStatus}; Exception: {Exception}",
                            totalAttempts,
                            maxAttempts,
                            result.Result?.StatusCode,
                            result.Exception);
                    });

            // configure the RestClient with the settings.
            return new RestClientOptions.Builder(options.BaseUri) // Common BaseUri for all outgoing requests for this client.
                .ConfigureResiliencePolicy(resiliencePolicy) // The resiliency policy for retrying
                .ConfigureHeader(options.ApiKey.HeaderKey, options.ApiKey.HeaderValue) // Configure common headers that will be sent for all outbound requests.
                .ConfigureTimeout(timeout) // Set the timeout for outgoing requests
                .ConfigureMiddlewareAsync(
                    async (restRequest, cancellationToken) =>
                    { // perform some kind of asynchronous action as middleware with every outgoing request.
                        await Task.Delay(1000, cancellationToken);
                        restRequest.UseHeader("x-jeremy-is", "asynchronously-active");
                        return restRequest;
                    })
                .ConfigureMiddleware(
                    restRequest =>
                    { // perform some kind of synchronous action as middleware with every outgoing request.
                        restRequest.UseHeader("x-jeremy-is", "synchronously-satisfying");
                        return restRequest;
                    })
                .Build(); // Use builder pattern to create the settings.
        }
    }
}