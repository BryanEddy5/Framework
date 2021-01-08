using System.Threading.Tasks;
using RestSharp;

namespace HumanaEdge.Webcore.Core.Testing.Client
{
    /// <summary>
    /// A client to be used for integration tests via Http requests.
    /// </summary>
    public interface ITestClient
    {
        /// <summary>
        /// Rest client interface.
        /// </summary>
        IRestClient RestClient { get; }

        /// <summary>
        /// Send an http request asynchronously.
        /// </summary>
        /// <param name="restRequest">The outbound request.</param>
        /// <returns>Response from client.</returns>
        Task<IRestResponse> ExecuteAsync(IRestRequest restRequest);

        /// <summary>
        /// Sends an http request asynchronously and returns a deserialized response.
        /// </summary>
        /// <param name="restRequest">The outbound request.</param>
        /// <typeparam name="TResponse">The shape of the response body.</typeparam>
        /// <returns>The rest response with the deserialized response.</returns>
        Task<IRestResponse<TResponse>> ExecuteAsync<TResponse>(IRestRequest restRequest);
    }
}