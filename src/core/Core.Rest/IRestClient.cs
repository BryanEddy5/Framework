using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    ///     The rest client sends RESTful http requests.
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        ///     Sends an http request without a body.
        /// </summary>
        /// <param name="restRequest">The http request information.</param>
        /// <param name="cancellationToken">The cancellation token for the request.</param>
        /// <returns>A http request response.</returns>
        Task<RestResponse> SendAsync(RestRequest restRequest, CancellationToken cancellationToken);

        /// <summary>
        ///     Sends an http request with a request Body.
        /// </summary>
        /// <param name="restRequest">The http request information.</param>
        /// <param name="cancellationToken">The cancellation token for the request.</param>
        /// <typeparam name="TRequest">The <see cref="Type" /> of the request body.</typeparam>
        /// <returns>A http request response.</returns>
        Task<RestResponse> SendAsync<TRequest>(RestRequest<TRequest> restRequest, CancellationToken cancellationToken);
    }
}