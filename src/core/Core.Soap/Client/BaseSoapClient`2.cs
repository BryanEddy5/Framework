using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using HumanaEdge.Webcore.Core.Soap.Client.Factory;
using HumanaEdge.Webcore.Core.Soap.Client.Models;
using HumanaEdge.Webcore.Core.Soap.Exceptions;

namespace HumanaEdge.Webcore.Core.Soap.Client
{
    /// <summary>
    /// The base class designed for SOAPey clients to inherit from.
    /// </summary>
    /// <typeparam name="TClient">The type inheriting from this base class.</typeparam>
    /// <typeparam name="TChannel">The service contract associated with this client.</typeparam>
    public abstract class BaseSoapClient<TClient, TChannel> : ClientBase<TChannel>
        where TChannel : class
        where TClient : BaseSoapClient<TClient, TChannel>
    {
        /// <summary>
        /// Constructor for when the <see cref="SoapClientOptions"/> is a static object.
        /// </summary>
        /// <param name="behaviorFactory">A factory pattern for generating an <see cref="IEndpointBehavior" />.</param>
        /// <param name="options">The client configuration.</param>
        protected BaseSoapClient(IEndpointBehaviorFactory behaviorFactory, SoapClientOptions options)
            : this(CreateEndpointConfiguration(options))
        {
            var soapEndpointBehavior = behaviorFactory.Create<TClient>(options);
            Endpoint.EndpointBehaviors.Add(soapEndpointBehavior);
        }

        /// <summary>
        /// Constructor for when the <see cref="SoapClientOptions"/> is a dynamic object.
        /// </summary>
        /// <param name="behaviorFactory">A factory pattern for generating an <see cref="IEndpointBehavior" />.</param>
        /// <param name="optionsFactory">A factory for obtaining the client configuration dynamically.</param>
        protected BaseSoapClient(IEndpointBehaviorFactory behaviorFactory, Func<SoapClientOptions> optionsFactory)
            : this(CreateEndpointConfiguration(optionsFactory()))
        {
            var options = optionsFactory();
            var soapEndpointBehavior = behaviorFactory.Create<TClient>(options);
            Endpoint.EndpointBehaviors.Add(soapEndpointBehavior);
        }

        /// <summary>
        /// Constructor for actually constructing this <see cref="BaseSoapClient{TClient,TChannel}"/>.
        /// </summary>
        /// <param name="endpointConfiguration">The configured options.</param>
        private BaseSoapClient(EndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration.Binding, endpointConfiguration.EndpointAddress)
        {
        }

        /// <summary>
        /// Constructs the <see cref="EndpointConfiguration"/> based on provided <see cref="SoapClientOptions"/>.
        /// Bindings and timeout setup is handled here.
        /// </summary>
        /// <param name="options">The client configuration.</param>
        /// <returns>The created <see cref="EndpointConfiguration"/>.</returns>
        private static EndpointConfiguration CreateEndpointConfiguration(SoapClientOptions options)
        {
            HttpBindingBase binding = options.BaseEndpoint.Scheme switch
            {
                "https" => new BasicHttpsBinding(),
                "http" => new BasicHttpBinding(),
                _ => throw new UnsupportedSchemeException(options.BaseEndpoint.Scheme)
            };

            binding.ReceiveTimeout = options.Timeout;

            binding.OpenTimeout = TimeSpan.FromSeconds(5);
            binding.SendTimeout = TimeSpan.FromSeconds(5);
            binding.CloseTimeout = TimeSpan.FromSeconds(5);

            var endpointAddress = new EndpointAddress(options.BaseEndpoint);
            return new EndpointConfiguration(binding, endpointAddress);
        }
    }
}