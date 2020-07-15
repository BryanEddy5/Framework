using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    internal sealed class InternalClient : IInternalClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="httpClient">A http client for sending http requests.</param>
        /// <param name="baseUri">The base path of the http request.</param>
        /// <param name="timeout">The length in time before the request is cancelled.</param>
        public InternalClient(HttpClient httpClient, Uri baseUri, TimeSpan timeout)
        {
            _httpClient = httpClient;
            Timeout = timeout;
            BaseUri = baseUri;
            _httpClient.BaseAddress = BaseUri;
            _httpClient.Timeout = Timeout;
        }

        /// <summary>
        /// The base path.
        /// </summary>
        public Uri BaseUri { get; }

        /// <summary>
        /// The length in time before the request times out.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage httpRequestMessage,
            CancellationToken cancellationToken)
        {
            return await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
        }
    }
}