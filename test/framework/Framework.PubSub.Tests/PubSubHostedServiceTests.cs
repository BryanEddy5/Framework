using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Subscription;
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
    /// Tests for <see cref="PubSubHostedService" />.
    /// </summary>
    public class PubSubHostedServiceTests : BaseTests
    {
        private readonly Mock<ISubOrchestrationService<Foo>> _subOrchestrationServiceMock;

        /// <summary>
        /// SUT.
        /// </summary>
        private readonly PubSubHostedService _pubSubHostedService;

        private readonly FakeSubscriberClient _subscriberClient;

        /// <summary>
        /// Ctor.
        /// </summary>
        public PubSubHostedServiceTests()
        {
            var logger = Moq.Create<ILogger<PubSubHostedService>>(MockBehavior.Loose);

            var config = FakeData.Create<PubSubOptions>();
            var optionsMock = Moq.Create<IOptionsMonitor<PubSubOptions>>();
            optionsMock.Setup(p => p.CurrentValue).Returns(config);

            _subOrchestrationServiceMock = Moq.Create<ISubOrchestrationService<Foo>>();

            _subscriberClient = new FakeSubscriberClient();
            var subscriberClientFactory = Moq.Create<ISubscriberClientFactory>();
            subscriberClientFactory.Setup(
                    s =>
                        s.GetSubscriberClient(new SubscriptionName(config.ProjectId, config.Name), config))
                .ReturnsAsync(_subscriberClient);

            _pubSubHostedService = new PubSubHostedService(
                logger.Object,
                optionsMock.Object,
                subscriberClientFactory.Object,
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
            _subOrchestrationServiceMock.Setup(x => x.ExecuteAsync(fakeFoo, CancellationToken.None))
                .Returns(Task.CompletedTask);
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo);

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
            _subOrchestrationServiceMock.Setup(m => m.ExecuteAsync(fakeFoo, CancellationToken.None))
                .Throws<Exception>();
            _subscriberClient.TestMessage = BuildPubsubMessage(fakeFoo);

            // act
            await _pubSubHostedService.StartAsync(CancellationToken.None);

            // assert
            _subscriberClient.TestReply.Should().Be(SubscriberClient.Reply.Nack);
        }

        private PubsubMessage BuildPubsubMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            return new PubsubMessage { Data = ByteString.CopyFrom(bytes) };
        }

        private PubsubMessage BuildPubsubMessage(Foo foo)
        {
            var json = JsonConvert.SerializeObject(
                foo,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            var bytes = Encoding.UTF8.GetBytes(json);
            return new PubsubMessage { Data = ByteString.CopyFrom(bytes) };
        }
    }
}