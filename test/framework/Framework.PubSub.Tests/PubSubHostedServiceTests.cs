using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests
{
    /// <summary>
    /// Tests for <see cref="BaseSubscriberHostedService{T}" />.
    /// </summary>
    public class PubSubHostedServiceTests : BaseTests
    {
        private readonly Mock<ISubOrchestrationService<Foo>> _subOrchestrationServiceMock;

        private readonly FakeSubscriberClient _subscriberClient;

        private readonly Mock<ILogger<PubSubHostedService>> _logger;

        private readonly Mock<ISubscriberClientFactory> _subscriberClientFactory;

        private readonly Mock<IOptionsMonitor<PubSubOptions>> _optionsMock;

        /// <summary>
        /// SUT.
        /// </summary>
        private PubSubHostedService _pubSubHostedService;

        /// <summary>
        /// Ctor.
        /// </summary>
        public PubSubHostedServiceTests()
        {
            _logger = Moq.Create<ILogger<PubSubHostedService>>(MockBehavior.Loose);

            var config = FakeData.Create<PubSubOptions>();
            _optionsMock = Moq.Create<IOptionsMonitor<PubSubOptions>>();
            _optionsMock.Setup(p => p.Get(typeof(Foo).FullName)).Returns(config);

            _subOrchestrationServiceMock = Moq.Create<ISubOrchestrationService<Foo>>();

            _subscriberClient = new FakeSubscriberClient();
            _subscriberClientFactory = Moq.Create<ISubscriberClientFactory>();
            _subscriberClientFactory.Setup(
                    s =>
                        s.GetSubscriberClient(new SubscriptionName(config.ProjectId, config.Name), config))
                .ReturnsAsync(_subscriberClient);

            _pubSubHostedService = new PubSubHostedService(
                _logger.Object,
                _optionsMock.Object,
                _subscriberClientFactory.Object,
                _subOrchestrationServiceMock.Object);
        }

        /// <summary>
        /// Testing that a path where serialization fails or null is passed returns an ack.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_InvalidMessage_ReturnAck()
        {
            // arrange
            _subscriberClient.TestMessage = BuildPubsubMessage("blahblahblah");
            var fooSubscriptionHandler = new FooSubscriptionHandler();

            _pubSubHostedService = new PubSubHostedService(
                _logger.Object,
                _optionsMock.Object,
                _subscriberClientFactory.Object,
                fooSubscriptionHandler);

            // act
            await _pubSubHostedService.StartAsync(CancellationTokenSource.Token);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Ack);
        }

        /// <summary>
        /// Testing the happy path for a message being sent and successfully processed.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_ValidMessage_SuccessfulProcess_ReturnAck()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);
            var fakeSubscriptionMessage = new SubscriptionMessage<Foo>(
                _subscriberClient.TestMessage.Data.ToByteArray(),
                new Lazy<Foo>(() => fakeFoo),
                fakeMessageId);
            _subOrchestrationServiceMock.Setup(x => x.ExecuteAsync(fakeSubscriptionMessage, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Ack);
        }

        /// <summary>
        /// Testing that a path where processing fails, and should be retried.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_ValidMessage_UnsuccessfulProcess_ReturnNack()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);
            var fakeSubscriptionMessage = new SubscriptionMessage<Foo>(
                _subscriberClient.TestMessage.Data.ToByteArray(),
                new Lazy<Foo>(() => fakeFoo),
                fakeMessageId);
            _subOrchestrationServiceMock.Setup(m => m.ExecuteAsync(fakeSubscriptionMessage, CancellationToken.None))
                .Throws<Exception>();

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Nack);
        }

        /// <summary>
        /// Validates the behavior of <see cref="BaseSubscriberHostedService{T}.StartAsync(CancellationToken)"/> when an exception is thrown with <see cref="Reply.Nack"/>.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_ValidMessage_ExceptionThrown_ReturnNack()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);
            var fakeSubscriptionMessage = new SubscriptionMessage<Foo>(
                _subscriberClient.TestMessage.Data.ToByteArray(),
                new Lazy<Foo>(() => fakeFoo),
                fakeMessageId);
            _subOrchestrationServiceMock.Setup(m => m.ExecuteAsync(fakeSubscriptionMessage, CancellationToken.None))
                .ThrowsAsync(new NackException("test"));

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Nack);
        }

        /// <summary>
        /// Validates the behavior of <see cref="BaseSubscriberHostedService{T}.StartAsync(CancellationToken)"/> when an exception is thrown with <see cref="Reply.Ack"/>.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_ValidMessage_ExceptionThrown_ReturnAck()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);
            var fakeSubscriptionMessage = new SubscriptionMessage<Foo>(
                _subscriberClient.TestMessage.Data.ToByteArray(),
                new Lazy<Foo>(() => fakeFoo),
                fakeMessageId);
            _subOrchestrationServiceMock.Setup(m => m.ExecuteAsync(fakeSubscriptionMessage, CancellationToken.None))
                .ThrowsAsync(new AckException("test"));

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Ack);
        }

        /// <summary>
        /// Validates the behavior of <see cref="BaseSubscriberHostedService{T}.StartAsync(CancellationToken)"/> is backwards compatible
        /// with <see cref="ISubOrchestrationService{TMessage}.ExecuteAsync(TMessage, CancellationToken)"/>.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task BackwardsCompatible()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);
            var fooSubscriptionHandler = new FooSubscriptionHandler();

            _pubSubHostedService = new PubSubHostedService(
                _logger.Object,
                _optionsMock.Object,
                _subscriberClientFactory.Object,
                fooSubscriptionHandler);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Ack);
        }

        private PubsubMessage BuildPubsubMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            return new PubsubMessage { Data = ByteString.CopyFrom(bytes) };
        }

        private PubsubMessage BuildPubsubMessage(Foo foo, string messageId)
        {
            var json = JsonConvert.SerializeObject(
                foo,
                StandardSerializerConfiguration.Settings);
            var bytes = Encoding.UTF8.GetBytes(json);
            return new PubsubMessage
            {
                Data = ByteString.CopyFrom(bytes),
                MessageId = messageId
            };
        }
    }
}