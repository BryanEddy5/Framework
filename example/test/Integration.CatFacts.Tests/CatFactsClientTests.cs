using AutoFixture;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Tests
{
    /// <summary>
    /// Unit tests for <see cref="CatFactsClient" />.
    /// </summary>
    public class CatFactsClientTests : BaseTests
    {
        private readonly Mock<ILogger<CatFactsClient>> _mockLogger;

        private readonly Mock<IOptionsSnapshot<CatFactsClientOptions>> _mockCatFactsClientOptions;

        private readonly Mock<IRestClientFactory> _mockRestClientFactory;

        /// <summary>
        /// System under test.
        /// </summary>
        private CatFactsClient? _catFactsClient;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public CatFactsClientTests()
        {
            _mockRestClientFactory = Moq.Create<IRestClientFactory>();
            CatFactsClientOptions options = FakeData.Build<CatFactsClientOptions>()
                .With(x => x.BaseUri, "https://humanaedge.com")
                .Create();
            options.Resilience.RetryAttempts = 6;
            _mockLogger = Moq.Create<ILogger<CatFactsClient>>();
            _mockCatFactsClientOptions = Moq.Create<IOptionsSnapshot<CatFactsClientOptions>>();
            _mockCatFactsClientOptions.Setup(x => x.Value).Returns(options);
        }

        /// <summary>
        /// Verifies the behavior of instantiating a <see cref="CatFactsClient" />.
        /// </summary>
        [Fact]
        public void ClientCreationTest()
        {
            // arrange act
            _catFactsClient = new CatFactsClient(_mockRestClientFactory.Object, _mockCatFactsClientOptions.Object, _mockLogger.Object);

            // assert
            Assert.NotNull(_catFactsClient);
        }
    }
}