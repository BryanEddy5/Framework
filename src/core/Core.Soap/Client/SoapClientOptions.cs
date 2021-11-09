using System;
using System.Collections.Generic;
using System.Net.Http;
using HumanaEdge.Webcore.Core.Soap.Client.Models;
using HumanaEdge.Webcore.Core.Soap.Exceptions;
using HumanaEdge.Webcore.Core.Soap.Resilience;
using Microsoft.Extensions.Primitives;
using Polly;

namespace HumanaEdge.Webcore.Core.Soap.Client
{
    /// <summary>
    /// Configuration options for a <see cref="BaseSoapClient{T,T}"/>.
    /// </summary>
    public sealed class SoapClientOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint.</param>
        /// <param name="headers">The default outgoing headers.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="resiliencePolicies">The resilience for this client.</param>
        /// <param name="soapHeaders">The SOAP headers.</param>
        private SoapClientOptions(
            Uri baseEndpoint,
            IDictionary<string, StringValues> headers,
            TimeSpan timeout,
            IAsyncPolicy<HttpResponseMessage>[] resiliencePolicies,
            IEnumerable<SoapHeader> soapHeaders)
        {
            BaseEndpoint = baseEndpoint;
            Headers = headers;
            Timeout = timeout;
            ResiliencePolicies = resiliencePolicies;
            SoapHeaders = soapHeaders;
        }

        /// <summary>
        /// The base uri for the endpoints this client communicates with.
        /// </summary>
        public Uri BaseEndpoint { get; }

        /// <summary>
        /// The headers that will be attached to all outgoing requests for this client.
        /// </summary>
        public IDictionary<string, StringValues> Headers { get; }

        /// <summary>
        /// The headers that will be created as SOAP Envelope headers for the out bound request.
        /// </summary>
        public IEnumerable<SoapHeader> SoapHeaders { get; }

        /// <summary>
        /// The timeout for http calls this client makes.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// A collection of resilience policies to apply to the client.
        /// </summary>
        public IAsyncPolicy<HttpResponseMessage>[] ResiliencePolicies { get; }

        /// <summary>
        /// A builder class for constructing <see cref="SoapClientOptions"/> fluently.
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// The base uri for the endpoints this client communicates with.
            /// </summary>
            private readonly Uri _baseEndpoint;

            /// <summary>
            /// The headers that will be attached to all outgoing requests for this client.
            /// </summary>
            private readonly Dictionary<string, StringValues> _defaultHeaders;

            /// <summary>
            /// The headers that will be created as SOAP Envelope headers for the out bound request.
            /// </summary>
            private IDictionary<string, SoapHeader> _soapHeaders;

            /// <summary>
            /// The timeout for http calls this client makes.
            /// </summary>
            private TimeSpan _timeout;

            /// <summary>
            /// A collection of resilience policies to apply to the client.
            /// </summary>
            private List<IAsyncPolicy<HttpResponseMessage>> _resiliencePolicies;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="baseEndpoint">The base endpoint.</param>
            public Builder(Uri baseEndpoint)
            {
                _baseEndpoint = baseEndpoint;
                _resiliencePolicies = new List<IAsyncPolicy<HttpResponseMessage>>
                {
                    ResiliencyPolicies.CircuitBreaker(TimeSpan.FromSeconds(5), 4),
                    ResiliencyPolicies.RetryWithExponentialBackoff(6)
                };
                _defaultHeaders = new Dictionary<string, StringValues>();
                _soapHeaders = new Dictionary<string, SoapHeader>();
                _timeout = TimeSpan.FromSeconds(8);
            }

            /// <summary>
            /// Alternative constructor accepting a string as the base endpoint instead.
            /// </summary>
            /// <param name="baseEndpoint">The base endpoint.</param>
            public Builder(string baseEndpoint)
                : this(new Uri(baseEndpoint))
            {
            }

            /// <summary>
            /// Generates <see cref="SoapClientOptions" /> from the fluent chained configuration.
            /// </summary>
            /// <returns><see cref="SoapClientOptions" />.</returns>
            public SoapClientOptions Build()
            {
                return new SoapClientOptions(
                    _baseEndpoint,
                    _defaultHeaders,
                    _timeout,
                    _resiliencePolicies.ToArray(),
                    _soapHeaders.Values);
            }

            /// <summary>
            /// Adds an HTTP header that will be applied to all outbound requests.<br/>
            /// Chain multiple times to add multiple headers.
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
            /// Adds an SOAP header that will be applied to all outbound requests.<br/>
            /// Chain multiple times to add multiple headers.
            /// </summary>
            /// <param name="soapHeader">The header value.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureSoapHeader(SoapHeader soapHeader)
            {
                if (_soapHeaders.ContainsKey(soapHeader.Name))
                {
                    throw new DuplicateSoapHeaderException(soapHeader);
                }

                _soapHeaders.Add(soapHeader.Name, soapHeader);

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
            /// Adds a resilience policy to the collection for this client.
            /// </summary>
            /// <param name="resiliencePolicy">The default resilience policy.</param>
            /// <returns>The builder instance, for fluent chaining.</returns>
            public Builder ConfigureResiliencePolicy(IAsyncPolicy<HttpResponseMessage> resiliencePolicy)
            {
                _resiliencePolicies.Add(resiliencePolicy);
                return this;
            }
        }
    }
}