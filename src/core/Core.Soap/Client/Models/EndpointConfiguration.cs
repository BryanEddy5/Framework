using System.ServiceModel;
using System.ServiceModel.Channels;

namespace HumanaEdge.Webcore.Core.Soap.Client.Models
{
    /// <summary>
    /// A combination of <see cref="Binding"/> and <see cref="EndpointAddress"/> constructed based on configuration.
    /// </summary>
    internal sealed class EndpointConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="endpointAddress">The endpoint address.</param>
        public EndpointConfiguration(Binding binding, EndpointAddress endpointAddress)
        {
            Binding = binding;
            EndpointAddress = endpointAddress;
        }

        /// <summary>
        /// The binding constructed by the configuration.
        /// </summary>
        public Binding Binding { get; }

        /// <summary>
        /// The endpoint address constructed by the configuration.
        /// </summary>
        public EndpointAddress EndpointAddress { get; }
    }
}