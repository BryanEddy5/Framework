using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using HumanaEdge.Webcore.Core.Common.Extensions;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Alerting;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Publication;
using HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests
{
    /// <summary>
    /// Unit tests for <see cref="PublisherClient{TMessage}"/>.
    /// </summary>
    public class PublisherClientTests : BaseTests
    {
        private Mock<IOptionsMonitor<PublisherOptions>> _optionsMock;

        private PublisherOptions _options;

        private Mock<IInternalPublisherClient> _internalPublisherClientMock;

        private Mock<IPublisherClientFactory> _publisherClientFactoryMock;

        private Mock<IPublishRequestConverter> _requestServiceMock;

        private Mock<ITelemetryFactory> _telemetryFactoryMock;

        private Mock<ILogger<PublisherClient<Foo>>> _loggerMock;

        private Mock<IPubsubAlertingService> _mockPubsubAlerting;

        private TopicName _fakeTopicName;

        /// <summary>
        /// System under test.
        /// </summary>
        private PublisherClient<Foo> _publisherClient;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public PublisherClientTests()
        {
            _optionsMock = Moq.Create<IOptionsMonitor<PublisherOptions>>();
            _options = FakeData.Create<PublisherOptions>();
            _fakeTopicName = new TopicName(_options.ProjectId, _options.TopicName);
            _internalPublisherClientMock = Moq.Create<IInternalPublisherClient>();
            _publisherClientFactoryMock = Moq.Create<IPublisherClientFactory>();
            _requestServiceMock = Moq.Create<IPublishRequestConverter>();
            _telemetryFactoryMock = Moq.Create<ITelemetryFactory>(MockBehavior.Loose);
            _loggerMock = Moq.Create<ILogger<PublisherClient<Foo>>>(MockBehavior.Loose);
            _mockPubsubAlerting = Moq.Create<IPubsubAlertingService>();

            _optionsMock.Setup(x => x.Get(typeof(Foo).FullName)).Returns(_options);
            _publisherClientFactoryMock.Setup(x => x.CreateAsync(_fakeTopicName, CancellationTokenSource.Token))
                .ReturnsAsync(_internalPublisherClientMock.Object);

            _publisherClient = new PublisherClient<Foo>(
                _optionsMock.Object,
                _loggerMock.Object,
                _publisherClientFactoryMock.Object,
                _requestServiceMock.Object,
                _mockPubsubAlerting.Object,
                _telemetryFactoryMock.Object);
        }

        /// <summary>
        /// Validates the behavior for <see cref="PublisherClient{TMessage}"/>.<br/>
        /// Ensures when all the stars align, publishing works as intended.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PublishAsync_Success()
        {
            // arrange
            var fakeMessages = FakeData.Create<Foo[]>();
            var fakeMessageIds = SetupPublishAsync(fakeMessages);

            // act
            var actual = await _publisherClient.PublishAsync(
                fakeMessages,
                CancellationTokenSource.Token);

            // assert
            actual.Should().BeEquivalentTo(fakeMessageIds);
        }

        /// <summary>
        /// Validates the behavior for <see cref="PublisherClient{TMessage}"/>.<br/>
        /// Ensures when exception is thrown by GCP, it is handled accordingly.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task PublishAsync_PublishException()
        {
            // arrange
            var fakeMessages = FakeData.Create<Foo[]>();
            SetupPublishAsync(fakeMessages, true);

            // act assert
            await Assert.ThrowsAsync<PublishException>(
                async () => await _publisherClient.PublishAsync(
                    fakeMessages,
                    CancellationTokenSource.Token));
        }

        /// <summary>
        /// Sets up the publisher client for basic interaction based on provided messages.
        /// </summary>
        /// <param name="messages">The provided messages to generate mock setups for.</param>
        /// <param name="throwException">Whether or not to throw exceptions.</param>
        /// <typeparam name="TMessage">The type of message.</typeparam>
        /// <returns>The fake responses from the publisher client.</returns>
        private IReadOnlyList<string> SetupPublishAsync<TMessage>(
            TMessage[] messages,
            bool throwException = false)
        {
            var publishRequests = new List<PublishRequest>();
            messages.ForEach(
                msg =>
                {
                    var fakePublishRequest = FakeData.Create<PublishRequest>();
                    _requestServiceMock
                        .Setup(x => x.Create(msg, _fakeTopicName))
                        .Returns(fakePublishRequest);
                    publishRequests.Add(fakePublishRequest);
                });
            var publishResponses = new List<PublishResponse>();
            publishRequests.ForEach(
                request =>
                {
                    var publishResponse = new PublishResponse
                    {
                        MessageIds = { FakeData.Create<string>() }
                    };
                    if (throwException == false)
                    {
                        _internalPublisherClientMock
                            .Setup(x => x.PublishAsync(request, CancellationTokenSource.Token))
                            .ReturnsAsync(publishResponse);
                        _mockPubsubAlerting
                            .Setup(alert => alert.IsPubsubAlert(_publisherClient.ClientAlertCondition, true))
                            .Returns(false);
                    }
                    else
                    {
                        _internalPublisherClientMock
                            .Setup(x => x.PublishAsync(request, CancellationTokenSource.Token))
                            .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "test")));
                        _mockPubsubAlerting
                            .Setup(alert => alert.IsPubsubAlert(_publisherClient.ClientAlertCondition, false))
                            .Returns(true);
                    }

                    publishResponses.Add(publishResponse);
                });
            return publishResponses.Select(x => x.MessageIds.FirstOrDefault()).ToArray();
        }
    }
}