using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Core.Rest.Resiliency;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Polly;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    ///    Configuration settings for outbound requests for the instance of <see cref="IRestClient"/>.
    /// </summary>
    public sealed class RestClientOptions : IRestFormattingSettings
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="baseUri">The base Uri for the outgoing request.</param>
        /// <param name="defaultHeaders">Optional. Default headers to be sent with all client http requests.</param>
        /// <param name="restRequestMiddleware">Optional. Transformations to be applied to the outgoing http request.</param>
        /// <param name="restRequestMiddlewareAsync">Optional. Asynchronous transformations to be applied to the outgoing http request.</param>
        /// <param name="timeout">Optional. The duration of which the request will timeout.</param>
        /// <param name="resiliencePolicies">Optional. Configured strategy for resiliency. Defaults to NoOp if omitted.</param>
        /// <param name="jsonSerializerSettings">Settings for JSON formatting.</param>
        /// <param name="bearerTokenFactory">A factory for generating a bearer token. </param>
        /// <param name="alertCondition">The default alert condition for this rest client.</param>
        private RestClientOptions(
            Uri baseUri,
            Dictionary<string, StringValues> defaultHeaders,
            RestRequestTransformation[] restRequestMiddleware,
            RestRequestTransformationAsync[] restRequestMiddlewareAsync,
            TimeSpan timeout,
            IAsyncPolicy<BaseRestResponse>[] resiliencePolicies,
            JsonSerializerSettings jsonSerializerSettings,
            (Func<CancellationToken, Task<string>> TokenFactory, string TokenKey)? bearerTokenFactory,
            AlertCondition<BaseRestResponse>? alertCondition)
        {
            BaseUri = baseUri;
            DefaultHeaders = defaultHeaders;
            RestRequestMiddleware = restRequestMiddleware;
            RestRequestMiddlewareAsync = restRequestMiddlewareAsync;
            Timeout = timeout;
            ResiliencePolicies = resiliencePolicies;
            JsonSerializerSettings = jsonSerializerSettings;
            BearerTokenFactory = bearerTokenFactory;
            AlertCondition = alertCondition;
        }

        /// <summary>
        /// Signature for configurable transformations on all outbound instances of <see cref="RestRequest" />.
        /// </summary>
        /// <param name="restRequest">The outbound request.</param>
        /// <returns>The transformed rest request.</returns>
        public delegate RestRequest RestRequestTransformation(RestRequest restRequest);

        /// <summary>
        /// Signature for configurable transformations on all async outbound instances of <see cref="RestRequest" />.
        /// </summary>
        /// <param name="restRequest">The outbound request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The transformed rest request.</returns>
        public delegate Task<RestRequest> RestRequestTransformationAsync(
            RestRequest restRequest,
            CancellationToken cancellationToken);

        /// <summary>
        /// The base URI of the request.
        /// </summary>
        public Uri BaseUri { get; }

        /// <summary>
        /// Optional headers to be applied to all outgoing requests for the of the client.
        /// </summary>
        public IReadOnlyDictionary<string, StringValues> DefaultHeaders { get; }

        /// <summary>
        /// Optional. Configured strategy for resiliency. Defaults to NoOp if omitted.
        /// </summary>
        public IAsyncPolicy<BaseRestResponse>[] ResiliencePolicies { get; }

        /// <summary>
        /// Optional. Transformations to be applied to the outgoing http request.
        /// </summary>
        public RestRequestTransformation[] RestRequestMiddleware { get; }

        /// <summary>
        /// Optional. Asynchronous transformations to be applied to the outgoing http request.
        /// </summary>
        public RestRequestTransformationAsync[] RestRequestMiddlewareAsync { get; }

        /// <summary>
        /// Optional. The duration of which the request will timeout.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// A factory for retrieving a bearer token.
        /// </summary>
        public (Func<CancellationToken, Task<string>> TokenFactory, string TokenKey)? BearerTokenFactory { get; }

        /// <summary>
        /// Settings for JSON formatting.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }

        /// <summary>
        /// The alert condition to execute that determines whether or not the telemetry associated with this
        /// rest request should be an alert or not.
        /// </summary>
        public AlertCondition<BaseRestResponse>? AlertCondition { get; }

        /// <summary>
        /// A builder pattern for generating <see cref="RestClientOptions" />.
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// The base URI of the request.
            /// </summary>
            private readonly Uri _baseUri;

            /// <summary>
            /// Optional headers to be applied to all outgoing requests for the of the client.
            /// </summary>
            private readonly Dictionary<string, StringValues> _defaultHeaders;

            /// <summary>
            /// Optional. Transformations to be applied to the outgoing http request.
            /// </summary>
            private readonly List<RestRequestTransformation> _restRequestMiddleware;

            /// <summary>
            /// Optional. Asynchronous transformations to be applied to the outgoing http request.
            /// </summary>
            private readonly List<RestRequestTransformationAsync> _restRequestMiddlewareAsync;

            /// <summary>
            /// Settings for JSON formatting.
            /// </summary>
            private JsonSerializerSettings _jsonSettings;

            /// <summary>
            /// Optional. Configured strategy for resiliency. Defaults to NoOp if omitted.
            /// </summary>
            private List<IAsyncPolicy<BaseRestResponse>> _resiliencePolicy;

            /// <summary>
            /// Optional. The duration of which the request will timeout.
            /// </summary>
            private TimeSpan _timeout;

            /// <summary>
            /// A factory for retrieving a bearer token.
            /// </summary>
            private (Func<CancellationToken, Task<string>> TokenFactory, string TokenKey)? _tokenFactory;

            /// <summary>
            /// The alert condition associated with the rest client itself.
            /// </summary>
            private AlertCondition<BaseRestResponse>? _alertCondition;

            /// <summary>
            /// A builder pattern for fluently producing <see cref="RestClientOptions" />.
            /// </summary>
            /// <param name="baseUri">The base Uri of the request.</param>
            public Builder(Uri baseUri)
            {
                _baseUri = baseUri;
                _defaultHeaders = new Dictionary<string, StringValues>();
                _restRequestMiddleware = new List<RestRequestTransformation>();
                _restRequestMiddlewareAsync = new List<RestRequestTransformationAsync>();
                _timeout = TimeSpan.FromSeconds(5);
                _jsonSettings = StandardSerializerConfiguration.Settings;
                _resiliencePolicy =
                    new List<IAsyncPolicy<BaseRestResponse>>(
                        new[] { ResiliencyPolicies.RetryWithExponentialBackoff(6) });
                _alertCondition = CommonRestAlertConditions.Standard();
            }

            /// <summary>
            /// Secondary constructor that accepts a string and converts to a <see cref="Uri" />.
            /// </summary>
            /// <param name="uri">The base URI of the client.</param>
            public Builder(string uri)
                : this(new Uri(uri))
            {
                // nop
            }

            /// <summary>
            /// Generates <see cref="RestClientOptions" /> from the fluent chained configuration.
            /// </summary>
            /// <returns><see cref="RestClientOptions" />.</returns>
            public RestClientOptions Build()
            {
                return new RestClientOptions(
                    _baseUri,
                    _defaultHeaders,
                    _restRequestMiddleware.ToArray(),
                    _restRequestMiddlewareAsync.ToArray(),
                    _timeout,
                    _resiliencePolicy.ToArray(),
                    _jsonSettings,
                    _tokenFactory,
                    _alertCondition);
            }

            /// <summary>
            /// Adds an HTTP header that will be applied to all outbound requests. May be invoked multiple times.
            /// </summary>
            /// <param name="headerKey">The header name.</param>
            /// <param name="headerValue">The header value.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureHeader(string headerKey, string headerValue)
            {
                if (_defaultHeaders.ContainsKey(headerKey))
                {
                    _defaultHeaders[headerKey] = StringValues.Concat(_defaultHeaders[headerKey], headerValue);
                }
                else
                {
                    _defaultHeaders[headerKey] = headerValue;
                }

                return this;
            }

            /// <summary>
            /// Configures JSON serializer settings for the client.
            /// </summary>
            /// <param name="jsonSerializerSettings">The JSON serializer settings for JSON formatting.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureJsonFormatting(JsonSerializerSettings jsonSerializerSettings)
            {
                _jsonSettings = jsonSerializerSettings;
                return this;
            }

            /// <summary>
            /// Applies a transformation to all outbound instances of <see cref="RestRequest" />.
            /// </summary>
            /// <param name="transformation">The transformation delegate.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureMiddleware(RestRequestTransformation transformation)
            {
                _restRequestMiddleware.Add(transformation);
                return this;
            }

            /// <summary>
            /// Applies an async transformation to all outbound instances of <see cref="RestRequest" />.
            /// </summary>
            /// <param name="transformationAsync">The asynchronous transformation.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureMiddlewareAsync(RestRequestTransformationAsync transformationAsync)
            {
                _restRequestMiddlewareAsync.Add(transformationAsync);
                return this;
            }

            /// <summary>
            /// Configures the default resilience policy for this client.
            /// </summary>
            /// <param name="resiliencePolicy">The default resilience policy.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureResiliencePolicy(IAsyncPolicy<BaseRestResponse> resiliencePolicy)
            {
                _resiliencePolicy.Add(resiliencePolicy);
                return this;
            }

            /// <summary>
            /// Configures a default timeout on all requests for the client.
            /// </summary>
            /// <param name="timeout">The timeout window.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureTimeout(TimeSpan timeout)
            {
                _timeout = timeout;
                return this;
            }

            /// <summary>
            /// Sets the bearer token based on a factory to generate it.
            /// </summary>
            /// <param name="tokenFactory">The factory for creating a token.</param>
            /// <typeparam name="TClient">The type of client.</typeparam>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureBearerToken<TClient>(Func<CancellationToken, Task<string>> tokenFactory)
            {
                _tokenFactory = (TokenFactory: tokenFactory, TokenKey: typeof(TClient).FullName !);
                _resiliencePolicy.Add(ResiliencyPolicies.RefreshToken<TClient>(tokenFactory, typeof(TClient).FullName !));
                return this;
            }

            /// <summary>
            /// Sets the default alert condition for this rest client.<br/>
            /// The default is <see cref="CommonRestAlertConditions.Minimum"/>.
            /// </summary>
            /// <param name="alertCondition">The alert condition to leverage for this client by default.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureAlertCondition(AlertCondition<BaseRestResponse> alertCondition)
            {
                _alertCondition = alertCondition;
                return this;
            }
        }
    }
}