using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using HumanaEdge.Webcore.Core.SecretsManager.Converters;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.SecretsManager.Clients;
using HumanaEdge.Webcore.Framework.SecretsManager.Tests.Stubs;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Tests
{
    /// <summary>
    /// Unit tests for <see cref="SecretsService{TSecret}"/>.
    /// </summary>
    public class SecretsServiceTests : BaseTests
    {
        /// <summary>
        /// System under test.
        /// </summary>
        private SecretsService<FakeSecret> _secretsService;

        private Mock<ISecretsHandler> _secretsHandlerMock;

        private Mock<IOptionsMonitor<SecretsOptions>> _optionsMock;

        private SecretsOptions _options;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public SecretsServiceTests()
        {
            _options = FakeData.Create<SecretsOptions>();
            _optionsMock = Moq.Create<IOptionsMonitor<SecretsOptions>>();
            _optionsMock.Setup(x => x.CurrentValue).Returns(_options);
            _secretsHandlerMock = Moq.Create<ISecretsHandler>();
            _secretsService = new SecretsService<FakeSecret>(_secretsHandlerMock.Object, _optionsMock.Object);
        }

        /// <summary>
        /// Validates the behavior of <see cref="SecretsService{T}.GetAsync(CancellationToken)"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetAsync()
        {
            // arrange
            var expected = FakeData.Create<FakeSecret>();
            _secretsHandlerMock
                .Setup(x => x.GetAsync<FakeSecret>(_options.ToSecretsKey(), CancellationTokenSource.Token))
                .ReturnsAsync(expected);

            // act
            var actual = await _secretsService.GetAsync(CancellationTokenSource.Token);

            // assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}