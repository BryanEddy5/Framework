using System.ServiceModel.Description;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Soap.Client.Factory;
using HumanaEdge.Webcore.Core.Soap.Tests.Stubs;
using HumanaEdge.Webcore.Core.Testing;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Core.Soap.Tests.Client
{
    /// <summary>
    /// Unit tests for <see cref="BaseSoapClient{TClient,TChannel}"/>.
    /// </summary>
    public class BaseSoapClientTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly IFooSoapClient _client;

        /// <summary>
        /// Mock of <see cref="IEndpointBehaviorFactory"/>.
        /// </summary>
        private readonly Mock<IEndpointBehaviorFactory> _mockEndpointBehaviorFactory;

        /// <summary>
        /// Mock of <see cref="IEndpointBehavior"/>.
        /// </summary>
        private readonly Mock<IEndpointBehavior> _mockEndpointBehavior;

        /// <summary>
        /// Real options populated with fake data.
        /// </summary>
        private SoapClientOptions _fakeOptions;

        /// <summary>
        /// Common test code.
        /// </summary>
        public BaseSoapClientTests()
        {
            _fakeOptions = new SoapClientOptions.Builder("https://foo.bar").Build();
            _mockEndpointBehavior = Moq.Create<IEndpointBehavior>();
            _mockEndpointBehaviorFactory = Moq.Create<IEndpointBehaviorFactory>();
            _mockEndpointBehaviorFactory
                .Setup(factory => factory.Create<FooSoapClient>(_fakeOptions))
                .Returns(_mockEndpointBehavior.Object);
            _client = new FooSoapClient(_mockEndpointBehaviorFactory.Object, _fakeOptions);
        }

        /// <summary>
        /// Verifies that the creation of <see cref="BaseSoapClient{TClient,TChannel}"/> works.
        /// </summary>
        [Fact]
        public void Client_CreatesSuccessfully()
        {
            _client.Should().NotBeNull();
        }
    }
}