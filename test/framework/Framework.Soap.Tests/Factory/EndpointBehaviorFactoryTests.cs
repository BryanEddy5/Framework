using System.ServiceModel.Description;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Soap.Client.Factory;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Soap.Contracts;
using HumanaEdge.Webcore.Framework.Soap.Factory;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;
using HumanaEdge.Webcore.Framework.Soap.Tests.Stubs;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Soap.Tests.Factory
{
    /// <summary>
    /// Unit tests for <see cref="EndpointBehaviorFactory"/>.
    /// </summary>
    public class EndpointBehaviorFactoryTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly IEndpointBehaviorFactory _endpointBehaviorFactory;

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
        public EndpointBehaviorFactoryTests()
        {
            _mockTelemetryFactory = Moq.Create<ITelemetryFactory>();
            _mockPollyContextFactory = Moq.Create<IPollyContextFactory>();
            _endpointBehaviorFactory = new EndpointBehaviorFactory(
                _mockTelemetryFactory.Object,
                _mockPollyContextFactory.Object);
        }

        /// <summary>
        /// Validates the behavior of <see cref="IEndpointBehaviorFactory"/>.<br/>
        /// Ensures new <see cref="EndpointBehavior"/>s are created successfully.
        /// </summary>
        [Fact]
        public void Create_CreatesSuccessfully()
        {
            // arrange
            var fakeSoapOptions = new SoapClientOptions.Builder("https://humana.com").Build();

            // act
            var actual = _endpointBehaviorFactory.Create<FooSoapClient>(fakeSoapOptions);

            // assert
            actual.Should().BeOfType<EndpointBehavior>();
        }
    }
}