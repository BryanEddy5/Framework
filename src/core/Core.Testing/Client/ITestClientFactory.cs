using RestSharp;

namespace HumanaEdge.Webcore.Core.Testing.Client
{
    /// <summary>
    /// A factory pattern for generating <see cref="IRestClient"/> and <see cref="IRestRequest"/>.
    /// </summary>
    public interface ITestClientFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ITestClient"/> and importing configuration.
        /// </summary>
        /// <param name="name">The name of the named client.</param>
        /// <returns>The client for sending restful requests.</returns>
        ITestClient Create(string name);
    }
}