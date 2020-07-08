using System;
using System.Net.Http;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    internal sealed class InternalClientFactory : IInternalClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InternalClientFactory" /> class.
        /// </summary>
        /// <param name="httpClientFactory"><see cref="IHttpClientFactory" /> for generating an <see cref="HttpClient" />.</param>
        public InternalClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public IInternalClient CreateClient(string clientName, Uri baseUri, TimeSpan timeout)
        {
            return new InternalClient(_httpClientFactory.CreateClient(clientName), baseUri, timeout);
        }
    }
}