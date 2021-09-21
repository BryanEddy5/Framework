using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Framework.Soap.Tests.Stubs
{
    /// <summary>
    /// A delegating handler that we can control for the sake of testing without making real http requests.
    /// </summary>
    public sealed class StubDelegatingHandler : DelegatingHandler
    {
        /// <summary>
        ///     A mock handler to be delegated to.
        /// </summary>
        private readonly IHttpMessageHandler _httpMessageHandler;

        /// <summary>
        ///     Designated constructor.
        /// </summary>
        /// <param name="httpMessageHandler">The mock HTTP handler to be delegated to.</param>
        public StubDelegatingHandler(IHttpMessageHandler httpMessageHandler)
        {
            _httpMessageHandler = httpMessageHandler;
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _httpMessageHandler.SendAsync(request, cancellationToken);
        }
    }
}