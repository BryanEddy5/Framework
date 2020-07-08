using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <summary>
    ///     An abstraction over the http client for sending requests.
    /// </summary>
    internal interface IInternalClient
    {
        /// <summary>
        ///     Send a http request.
        /// </summary>
        /// <param name="httpRequestMessage">The http request message to be sent.</param>
        /// <param name="cancellationToken">The request cancellation token.</param>
        /// <returns>The response from the http request.</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);
    }
}