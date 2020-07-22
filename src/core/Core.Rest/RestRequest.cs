using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Primitives;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// Request meta data for an http request.
    /// </summary>
    public class RestRequest
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="relativePath">The relative path for the http request.</param>
        /// <param name="httpMethod">The http method for the http request.</param>
        public RestRequest(string relativePath, HttpMethod httpMethod)
        {
            RelativePath = relativePath;
            HttpMethod = httpMethod;
            Headers = new Dictionary<string, StringValues>();
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
        /// The relative path for the http request.
        /// </summary>
        public string RelativePath { get; }

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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is RestRequest that))
            {
                return false;
            }

            var a1 = Headers.ToArray();
            var a2 = that.Headers.ToArray();

            var areEqual = a1.Length == a2.Length && !a1.Except(a2).Any();

            return (that.RelativePath == RelativePath) && (that.HttpMethod == HttpMethod) && areEqual;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(RelativePath, HttpMethod, Headers);
        }
    }
}