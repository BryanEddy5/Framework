using System.Collections.Generic;
using System.Net.Http;
using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// Request meta data for an http request.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public class RestRequest
    {
        /// <summary>
        /// The query parameters to create the query string for the HTTP request.
        /// </summary>
        private readonly IDictionary<string, string> _queryParams;

        /// <summary>
        /// Path to the resource, relative to the base URI.
        /// </summary>
        private readonly string _relativePath;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="relativePath">The relative path for the http request.</param>
        /// <param name="httpMethod">The http method for the http request.</param>
        public RestRequest(string relativePath, HttpMethod httpMethod)
        {
            _relativePath = relativePath;
            HttpMethod = httpMethod;
            Headers = new Dictionary<string, StringValues>();
            _queryParams = new Dictionary<string, string>();
        }

        /// <summary>
        /// Headers associated with the http request.
        /// </summary>
        public Dictionary<string, StringValues> Headers { get; }

        /// <summary>
        /// The http method for the http request.
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Path to the resource, relative to the base URI.
        /// </summary>
        public string RelativePath =>
            _queryParams.Count > 0
                ? QueryHelpers.AddQueryString(_relativePath, _queryParams)
                : _relativePath;

        /// <summary>
        /// The alert condition to execute that determines whether or not the telemetry associated with this
        /// rest request should be an alert or not.
        /// </summary>
        public AlertCondition? AlertCondition { get; private set; }

        /// <summary>
        /// Add query string parameters to create a query string for the request.
        /// </summary>
        /// <param name="field">field to be queried.</param>
        /// <param name="value">value to be queried against. Separate commas between values for a collection.</param>
        /// <returns>The same instance for fluent chaining.</returns>
        public RestRequest AddQueryParams(string field, string value)
        {
            if (!_queryParams.ContainsKey(field))
            {
                _queryParams.Add(field, value);
            }
            else
            {
                _queryParams[field] = string.Join(",", _queryParams[field], value);
            }

            return this;
        }

        /// <summary>
        /// Add query string parameters to create a query string for the request.
        /// </summary>
        /// <param name="queryParams">A dictionary to be converted to a query string.</param>
        /// <returns>The same instance for fluent chaining.</returns>
        public RestRequest AddQueryParams(IDictionary<string, string> queryParams)
        {
            foreach (var kvp in queryParams)
            {
                if (_queryParams.ContainsKey(kvp.Key))
                {
                    _queryParams[kvp.Key] = string.Join(",", _queryParams[kvp.Key], kvp.Value);
                }
                else
                {
                    _queryParams.Add(kvp.Key, kvp.Value);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds an Accept header to the http request that indicates the the response's MIME-type should be.
        /// The header meets the https://tools.ietf.org/html/rfc7231#section-5.3.2 spec.
        /// </summary>
        /// <param name="mediaType">The mediaType of the request header.</param>
        /// <returns><see cref="RestRequest" /> for fluent chaining.</returns>
        public RestRequest UseAcceptHeader(MediaType mediaType)
        {
            var acceptsKey = "Accept";
            if (Headers.TryGetValue(acceptsKey, out var headerValue))
            {
                var accept = StringValues.Concat(headerValue, mediaType.MimeType);
                Headers[acceptsKey] = accept;
            }
            else
            {
                Headers.Add(acceptsKey, new StringValues(mediaType.MimeType));
            }

            return this;
        }

        /// <summary>
        /// Add headers to the request.
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The mediaType of the request header.</param>
        /// <returns><see cref="RestRequest" /> for fluent chaining.</returns>
        public RestRequest UseHeader(string key, string value)
        {
            if (Headers.TryGetValue(key, out var headerValue))
            {
                var accept = StringValues.Concat(headerValue, value);
                Headers[key] = accept;
            }
            else
            {
                Headers.Add(key, new StringValues(value));
            }

            return this;
        }

        /// <summary>
        /// Configures the alert condition for this request.
        /// </summary>
        /// <param name="alertCondition">The alert condition.</param>
        /// <returns><see cref="RestRequest" /> for fluent chaining.</returns>
        /// <remarks>
        /// If you want to throw a custom exception, be sure to assign <see cref="AlertCondition"/>'s Exception
        /// property with the exception you want thrown. This will become the inner exception of the custom
        /// <see cref="AlertConditionMetException"/> if it gets thrown.
        /// </remarks>
        public RestRequest ConfigureAlertCondition(AlertCondition alertCondition)
        {
            AlertCondition = alertCondition;
            return this;
        }
    }
}