using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HumanaEdge.Webcore.Framework.Soap.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HttpRequestMessage"/>s.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Allows adding a new header, or updating an existing one, with more string values.
        /// </summary>
        /// <param name="headers">The headers to inspect.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The modified headers.</returns>
        /// <remarks>
        /// This encapsulates header management. Unlike the RestClient, this works directly with the
        /// <see cref="HttpRequestMessage"/>, which has immutable and not-very-friend-to-work-with headers.
        /// </remarks>
        public static HttpRequestHeaders SetHeader(this HttpRequestHeaders headers, string key, string value)
        {
            var localHeaders = new List<string>();
            if (headers.TryGetValues(key, out var values))
            {
                localHeaders.AddRange(values);
                headers.Remove(key);
            }

            localHeaders.Add(value);
            headers.Add(key, localHeaders);
            return headers;
        }
    }
}