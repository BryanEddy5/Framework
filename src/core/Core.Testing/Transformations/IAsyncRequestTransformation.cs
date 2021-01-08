using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace HumanaEdge.Webcore.Core.Testing.Transformations
{
    /// <summary>
    /// An asynchronous service that transforms the rest request.
    /// </summary>
    public interface IAsyncRequestTransformation : INamedClientService
    {
        /// <summary>
        /// Executes the transformation.
        /// </summary>
        /// <param name="request">The outbound request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(IRestRequest request, CancellationToken cancellationToken = default);
    }
}