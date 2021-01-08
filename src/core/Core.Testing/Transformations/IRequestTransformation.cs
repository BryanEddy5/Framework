using RestSharp;

namespace HumanaEdge.Webcore.Core.Testing.Transformations
{
    /// <summary>
    /// A service that transforms the rest request.
    /// </summary>
    public interface IRequestTransformation : INamedClientService
    {
        /// <summary>
        /// Executes the request transformation.
        /// </summary>
        /// <param name="request">The request to be transformed.</param>
        void Execute(IRestRequest request);
    }
}