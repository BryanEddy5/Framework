using System;

namespace HumanaEdge.Webcore.Core.Testing.Client
{
    /// <summary>
    /// Configuration options for the <see cref="ITestClient"/>.
    /// </summary>
    public class TestClientOptions
    {
        /// <summary>
        /// The base url of the service to be tested.
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// An array of headers to be sent with the outbound http request.
        /// </summary>
        public Header[] Headers { get; set; } = Array.Empty<Header>();

        /// <summary>
        /// Key/Value pairs that are headers for the outbound http request.
        /// </summary>
        public class Header
        {
            /// <summary>
            /// The header key.
            /// </summary>
            public string? Key { get; set; }

            /// <summary>
            /// The header value.
            /// </summary>
            public string? Value { get; set; }
        }
    }
}