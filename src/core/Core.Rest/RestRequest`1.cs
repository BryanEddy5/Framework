using System;
using System.Net.Http;
using Microsoft.Extensions.Primitives;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// A rest request that contains a body.
    /// </summary>
    /// <typeparam name="TBody">The <see cref="Type" /> of the body for the request.</typeparam>
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class RestRequest<TBody> : RestRequest
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="relativePath">The relative path for the http request.</param>
        /// <param name="httpMethod">The http method for the http request.</param>
        /// <param name="body">The body of the rest request.</param>
        /// <param name="mediaType">The media type associated with the request.</param>
        public RestRequest(string relativePath, HttpMethod httpMethod, TBody body, MediaType mediaType)
            : base(relativePath, httpMethod)
        {
            RequestBody = body;
            MediaType = mediaType;
        }

        /// <summary>
        /// The <see cref="MediaType"/> that the body is serialized to.
        /// </summary>
        public MediaType MediaType { get; }

        /// <summary>
        /// The request body of the rest request.
        /// </summary>
        public TBody RequestBody { get; }

        /// <summary>
        /// Add headers to the request.
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The mediaType of the request header.</param>
        /// <returns><see cref="RestRequest" /> for fluent chaining.</returns>
        public new RestRequest<TBody> UseHeader(string key, string value)
        {
            base.UseHeader(key, value);
            return this;
        }
    }
}