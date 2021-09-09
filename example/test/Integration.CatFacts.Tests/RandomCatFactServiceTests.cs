using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Client;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Client.Contracts;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Converter;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Exceptions;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Services;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Tests
{
    /// <summary>
    /// Unit tests for <see cref="RandomCatFactService" />.
    /// </summary>
    public class RandomCatFactServiceTests : BaseTests
    {
        /// <summary>
        /// A mock rest client.
        /// </summary>
        private readonly Mock<ICatFactsClient> _mockClient;

        /// <summary>
        /// System under test.
        /// </summary>
        private readonly RandomCatFactService _randomCatFactService;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public RandomCatFactServiceTests()
        {
            _mockClient = Moq.Create<ICatFactsClient>();
            _randomCatFactService = new RandomCatFactService(_mockClient.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RandomCatFactService.GetAsync" />.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task ExecuteAsync()
        {
            // arrange
            var fakeRequest = new RestRequest(
                RandomCatFactService.RelativePath,
                HttpMethod.Get);

            var fakeCatFactsResponse = FakeData.Create<RandomCatFactsResponse>();
            var fakeResponseBytes = FakeData.Create<byte[]>();
            var fakeResponse = new RestResponse(
                true,
                new TestRestResponseDeserializer(x => fakeCatFactsResponse, fakeResponseBytes),
                HttpStatusCode.OK);
            _mockClient
                .Setup(x => x.SendAsync(
                    fakeRequest,
                    CancellationTokenSource.Token))
                .ReturnsAsync(fakeResponse);
            var expected = fakeCatFactsResponse.ToCatFact();

            // act
            var actual = await _randomCatFactService.GetAsync(CancellationTokenSource.Token);

            // assert
            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RandomCatFactService.GetAsync" /> when a random cat fact
        /// cannot be found.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task ExecuteAsync_NotFoundException()
        {
            // arrange
            var fakeRequest = new RestRequest(
                RandomCatFactService.RelativePath,
                HttpMethod.Get);

            var fakeCatFactsResponse = FakeData.Create<RandomCatFactsResponse>();
            var fakeResponseBytes = FakeData.Create<byte[]>();
            var fakeResponse = new RestResponse(
                false,
                new TestRestResponseDeserializer(x => fakeCatFactsResponse, fakeResponseBytes),
                HttpStatusCode.NotFound);
            _mockClient
                .Setup(x => x.SendAsync(fakeRequest, CancellationTokenSource.Token))
                .ReturnsAsync(fakeResponse);

            // act assert
            await Assert.ThrowsAsync<NotFoundCatFactsExceptions>(
                () => _randomCatFactService.GetAsync(CancellationTokenSource.Token));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RandomCatFactService.GetAsync" /> when a random cat fact
        /// response body is empty.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task ExecuteAsync_NoBody_NotFoundExceptionThrown()
        {
            // arrange
            var fakeRequest = new RestRequest(
                RandomCatFactService.RelativePath,
                HttpMethod.Get);

            var fakeCatFactsResponse = FakeData.Create<RandomCatFactsResponse>();
            var fakeResponseBytes = Array.Empty<byte>();
            var fakeResponse = new RestResponse(
                true,
                new TestRestResponseDeserializer(x => fakeCatFactsResponse, fakeResponseBytes),
                HttpStatusCode.OK);
            _mockClient.Setup(x => x.SendAsync(fakeRequest, CancellationTokenSource.Token))
                .ReturnsAsync(fakeResponse);

            // act assert
            await Assert.ThrowsAsync<NotFoundCatFactsExceptions>(
                () => _randomCatFactService.GetAsync(CancellationTokenSource.Token));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RandomCatFactService.GetAsync" /> when the integration
        /// fails for another reason.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task ExecuteAsync_InternalError()
        {
            // arrange
            var fakeRequest = new RestRequest(
                RandomCatFactService.RelativePath,
                HttpMethod.Get);

            var fakeCatFactsResponse = FakeData.Create<RandomCatFactsResponse>();
            var fakeResponseBytes = FakeData.Create<byte[]>();
            var fakeResponse = new RestResponse(
                false,
                new TestRestResponseDeserializer(x => fakeCatFactsResponse, fakeResponseBytes),
                HttpStatusCode.InternalServerError);
            _mockClient.Setup(x => x.SendAsync(fakeRequest, CancellationTokenSource.Token))
                .ReturnsAsync(fakeResponse);

            // act assert
            await Assert.ThrowsAsync<CatFactsException>(
                () => _randomCatFactService.GetAsync(CancellationTokenSource.Token));
        }
    }
}