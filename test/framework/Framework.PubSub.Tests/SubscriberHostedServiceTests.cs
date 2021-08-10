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
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Factory;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware.Builder;
using HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using JsonException = Newtonsoft.Json.JsonException;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests
{
    /// <summary>
    /// Tests for <see cref="SubscriberHostedService{TMessage}" />.
    /// </summary>
    public class SubscriberHostedServiceTests : BaseTests
    {
        private FakeSubscriberClient _subscriberClient;

        private Mock<ILogger<SubscriberHostedService<Foo>>> _logger;

        private Mock<ISubscriberClientFactory> _subscriberClientFactory;

        private Mock<IMiddlewareBuilder<Foo>> _middlewareBuilderMock;

        /// <summary>
        /// SUT.
        /// </summary>
        private SubscriberHostedService<Foo> _pubSubHostedService;

        /// <summary>
        /// Testing that a path where serialization fails or null is passed returns an ack.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_InvalidMessage_ReturnAck()
        {
            // arrange
            Setup();
            _subscriberClient.TestMessage = BuildPubsubMessage("blahblahblah");

            _pubSubHostedService = new SubscriberHostedService<Foo>(
                _logger.Object,
                _subscriberClientFactory.Object,
                _middlewareBuilderMock.Object);

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
            Setup();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);

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
            Setup(context => throw new Exception());
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Nack);
        }

        /// <summary>
        /// Testing that a path where processing fails, and should be retried.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_JsonParsingException_ReturnAck()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            Setup(context => throw new JsonParsingException(new JsonException()));
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Ack);
        }

        /// <summary>
        /// Validates the behavior of <see cref="SubscriberHostedService{TMessage}.StartAsync(CancellationToken)"/> when an exception is thrown with <see cref="Reply.Nack"/>.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_ValidMessage_ExceptionThrown_ReturnNack()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            Setup(context => throw new NackException("shit broke and that ain't cool"));
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Nack);
        }

        /// <summary>
        /// Validates the behavior of <see cref="SubscriberHostedService{TMessage}.StartAsync(CancellationToken)"/> when an exception is thrown with <see cref="Reply.Ack"/>.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task StartAsync_ValidMessage_ExceptionThrown_ReturnAck()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            Setup(context => throw new AckException("shit broke but that's okay"));
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Ack);
        }

        /// <summary>
        /// Validates the behavior of <see cref="SubscriberHostedService{TMessage}.StartAsync(CancellationToken)"/> is backwards compatible
        /// with <see cref="ISubOrchestrationService{TMessage}.ExecuteAsync(TMessage, CancellationToken)"/>.
        /// </summary>
        /// <returns>A task.</returns>
        [Fact]
        public async Task BackwardsCompatible()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeMessageId = FakeData.Create<string>();
            Setup();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo, fakeMessageId);

            _pubSubHostedService = new SubscriberHostedService<Foo>(
                _logger.Object,
                _subscriberClientFactory.Object,
                _middlewareBuilderMock.Object);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Ack);
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private void Setup(MessageDelegate messageDelegate = null)
        {
            messageDelegate ??= context => Task.CompletedTask;
            _logger = Moq.Create<ILogger<SubscriberHostedService<Foo>>>(MockBehavior.Loose);
            _middlewareBuilderMock = Moq.Create<IMiddlewareBuilder<Foo>>();
            _middlewareBuilderMock.Setup(x => x.Build()).Returns(messageDelegate);

            _subscriberClient = new FakeSubscriberClient();
            _subscriberClientFactory = Moq.Create<ISubscriberClientFactory>();
            _subscriberClientFactory.Setup(
                    s =>
                        s.Create(typeof(Foo).FullName))
                .ReturnsAsync(_subscriberClient);

            _pubSubHostedService = new SubscriberHostedService<Foo>(
                _logger.Object,
                _subscriberClientFactory.Object,
                _middlewareBuilderMock.Object);
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