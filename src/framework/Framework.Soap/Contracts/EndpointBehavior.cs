using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;

namespace HumanaEdge.Webcore.Framework.Soap.Contracts
{
    /// <inheritdoc />
    /// <remarks>
    /// Our custom logic that handles all the fancy stuff like telemetry and logging and supports
    /// the various configurations we offer via <see cref="SoapClientOptions"/>.
    /// </remarks>
    internal sealed class EndpointBehavior : IClientMessageInspector, IEndpointBehavior
    {
        /// <summary>
        /// The client name.
        /// </summary>
        private readonly string _clientName;

        /// <summary>
        /// The client configuration.
        /// </summary>
        private readonly SoapClientOptions _soapClientOptions;

        /// <inheritdoc cref="ITelemetryFactory"/>
        private readonly ITelemetryFactory _telemetryFactory;

        /// <inheritdoc cref="IPollyContextFactory"/>
        private readonly IPollyContextFactory _pollyContextFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clientName">The client name.</param>
        /// <param name="soapClientOptions">The client configuration.</param>
        /// <param name="telemetryFactory">The factory for generating telemetry.</param>
        /// <param name="pollyContextFactory">The factory for generating polly-contexts.</param>
        public EndpointBehavior(
            string clientName,
            SoapClientOptions soapClientOptions,
            ITelemetryFactory telemetryFactory,
            IPollyContextFactory pollyContextFactory)
        {
            _clientName = clientName;
            _soapClientOptions = soapClientOptions;
            _telemetryFactory = telemetryFactory;
            _pollyContextFactory = pollyContextFactory;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This injects a <see cref="SoapHttpMessageHandler"/> in to support our extra functionality.
        /// </remarks>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            bindingParameters.Add(
                new Func<HttpClientHandler, HttpMessageHandler>(
                    httpClientHandler => new SoapHttpMessageHandler(
                        _clientName,
                        httpClientHandler,
                        _soapClientOptions,
                        _telemetryFactory,
                        _pollyContextFactory)));
        }

        /// <inheritdoc />
        /// <remarks>Unused by this service, but required by implementing <see cref="IEndpointBehavior"/>.</remarks>
        [ExcludeFromCodeCoverage]
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(this);
        }

        /// <inheritdoc />
        /// <remarks>Unused by this service, but required by implementing <see cref="IEndpointBehavior"/>.</remarks>
        [ExcludeFromCodeCoverage]
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        /// <inheritdoc />
        /// <remarks>Unused by this service, but required by implementing <see cref="IEndpointBehavior"/>.</remarks>
        [ExcludeFromCodeCoverage]
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        /// <inheritdoc />
        /// <remarks>Unused by this service, but required by implementing <see cref="IClientMessageInspector"/>.</remarks>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <inheritdoc/>
        /// <remarks>Attaches SOAP envelope headers.</remarks>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            foreach (var header in _soapClientOptions.SoapHeaders)
            {
                request.Headers.Add(MessageHeader.CreateHeader(header.Name, header.NameSpace, header.Value));
            }

            return request;
        }
    }
}