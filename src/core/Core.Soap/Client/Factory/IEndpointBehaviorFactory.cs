using System.ServiceModel.Description;

namespace HumanaEdge.Webcore.Core.Soap.Client.Factory
{
    /// <summary>
    /// A factory for generating <see cref="IEndpointBehavior" />s.
    /// </summary>
    public interface IEndpointBehaviorFactory
    {
        /// <summary>
        /// Creates an <see cref="IEndpointBehavior" /> that will support the given <see cref="SoapClientOptions" />.
        /// </summary>
        /// <typeparam name="TClient">The most-derived type of the client that the endpoint behavior will be configured for.</typeparam>
        /// <param name="soapClientOptions">The client options.</param>
        /// <returns>
        /// An <see cref="IEndpointBehavior" /> that can be attached to the <see cref="BaseSoapClient{TClient, TChannel}" /> to
        /// support the configured <see cref="SoapClientOptions" />.
        /// </returns>
        IEndpointBehavior Create<TClient>(SoapClientOptions soapClientOptions);
    }
}