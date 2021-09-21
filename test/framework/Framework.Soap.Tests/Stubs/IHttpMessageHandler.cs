using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Framework.Soap.Tests.Stubs
{
    /// <summary>
    /// An interface that mirrors the implementation of the basic <see cref="HttpMessageHandler"/>.
    /// This enables greater control over the testability of our <see cref="SoapHttpMessageHandler"/>.
    /// </summary>
    public interface IHttpMessageHandler
    {
        /// <summary>
        /// Sends an HTTP message asynchronously.
        /// </summary>
        /// <param name="httpRequestMessage">The message to send.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The HTTP response.</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);
    }
}