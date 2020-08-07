using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        /// <param name="timeout">Optional. The duration of which the request will timeout.</param>
        /// <param name="resiliencePolicy">Optional. Configured strategy for resiliency. Defaults to NoOp if omitted.</param>
        /// <param name="jsonSerializerSettings">Settings for JSON formatting.</param>
        private RestClientOptions(
            Uri baseUri,
            Dictionary<string, StringValues> defaultHeaders,
            RestRequestTransformation[] restRequestMiddleware,
            TimeSpan timeout,
            IAsyncPolicy<BaseRestResponse> resiliencePolicy,
            JsonSerializerSettings jsonSerializerSettings)
        {
            BaseUri = baseUri;
            DefaultHeaders = defaultHeaders;
            RestRequestMiddleware = restRequestMiddleware;
            Timeout = timeout;
            ResiliencePolicy = resiliencePolicy;
            JsonSerializerSettings = jsonSerializerSettings;
        }

        /// <summary>
        /// Signature for configurable transformations on all outbound instances of <see cref="RestRequest" />.
        /// </summary>
        /// <param name="restRequest">The outbound request.</param>
        /// <returns>The transformed rest request.</returns>
        public delegate RestRequest RestRequestTransformation(RestRequest restRequest);

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
        public IAsyncPolicy<BaseRestResponse> ResiliencePolicy { get; }

        /// <summary>
        /// Optional. Transformations to be applied to the outgoing http request.
        /// </summary>
        public RestRequestTransformation[] RestRequestMiddleware { get; }

        /// <summary>
        /// Optional. The duration of which the request will timeout.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Settings for JSON formatting.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; }

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
            /// Settings for JSON formatting.
            /// </summary>
            private JsonSerializerSettings _jsonSettings;

            /// <summary>
            /// Optional. Configured strategy for resiliency. Defaults to NoOp if omitted.
            /// </summary>
            private IAsyncPolicy<BaseRestResponse> _resiliencePolicy;

            /// <summary>
            /// Optional. The duration of which the request will timeout.
            /// </summary>
            private TimeSpan _timeout;

            /// <summary>
            /// A builder pattern for fluently producing <see cref="RestClientOptions" />.
            /// </summary>
            /// <param name="baseUri">The base Uri of the request.</param>
            public Builder(Uri baseUri)
            {
                _baseUri = baseUri;
                _defaultHeaders = new Dictionary<string, StringValues>();
                _restRequestMiddleware = new List<RestRequestTransformation>();
                _timeout = TimeSpan.FromSeconds(5);
                _resiliencePolicy = Policy.NoOpAsync<BaseRestResponse>();
                _jsonSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
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
                    _timeout,
                    _resiliencePolicy,
                    _jsonSettings);
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
            /// Configures the default resilience policy for this client.
            /// </summary>
            /// <param name="resiliencePolicy">The default resilience policy.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureResiliencePolicy(IAsyncPolicy<BaseRestResponse> resiliencePolicy)
            {
                _resiliencePolicy = resiliencePolicy;
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
        }
    }
}