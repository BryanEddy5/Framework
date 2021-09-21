using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Soap.Contracts;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;
using HumanaEdge.Webcore.Framework.Soap.Tests.Stubs;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Soap.Tests.Contracts
{
    /// <summary>
    /// Unit tests for <see cref="EndpointBehavior"/>.
    /// </summary>
    public class EndpointBehaviorTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly IEndpointBehavior _endpointBehavior;

        /// <summary>
        /// Mock of <see cref="ITelemetryFactory"/>.
        /// </summary>
        private readonly Mock<ITelemetryFactory> _mockTelemetryFactory;

        /// <summary>
        /// Mock of <see cref="IPollyContextFactory"/>.
        /// </summary>
        private readonly Mock<IPollyContextFactory> _mockPollyContextFactory;

        /// <summary>
        /// Common setup code.
        /// </summary>
        public EndpointBehaviorTests()
        {
            _mockTelemetryFactory = Moq.Create<ITelemetryFactory>();
            _mockPollyContextFactory = Moq.Create<IPollyContextFactory>();
            var fakeOptions = new SoapClientOptions.Builder("https://humana.com").Build();
            _endpointBehavior = new EndpointBehavior(
                FakeData.Create<string>(),
                fakeOptions,
                _mockTelemetryFactory.Object,
                _mockPollyContextFactory.Object);
        }

        /// <summary>
        /// Validates the behavior of <see cref="EndpointBehavior"/>s.<br/>
        /// Ensures our <see cref="SoapHttpMessageHandler"/> is properly added as expected.<br/>
        /// Also ensures that the func being executed does indeed return a <see cref="SoapHttpMessageHandler"/>
        /// as expected.
        /// </summary>
        [Fact]
        public void AddBindingParameters_ProducesSoapHttpMessageHandler()
        {
            // arrange
            var serviceEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(IBarSoapReference)));
            var bindingParameters = new BindingParameterCollection();

            // act
            _endpointBehavior.AddBindingParameters(serviceEndpoint, bindingParameters);
            var actual = ((Func<HttpClientHandler, HttpMessageHandler>)bindingParameters[0])(new HttpClientHandler());

            // assert
            bindingParameters.Count.Should().Be(1);
            bindingParameters.First()!.Should().BeOfType<Func<HttpClientHandler, HttpMessageHandler>>();
            actual.Should().BeOfType<SoapHttpMessageHandler>();
        }
    }
}