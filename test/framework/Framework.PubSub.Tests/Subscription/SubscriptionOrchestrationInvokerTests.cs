using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Converters;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware;
using HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Subscription
{
    /// <summary>
    /// Unit tests for <see cref="SubscriptionOrchestrationInvoker{TMessage}"/>.
    /// </summary>
    public class SubscriptionOrchestrationInvokerTests : BaseTests
    {
        private readonly Mock<ISubOrchestrationService<Foo>> _subOrchestrationServiceMock;

        private readonly Mock<IServiceProvider> _serviceProviderMock;

        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;

        private readonly Mock<IServiceScope> _serviceScopeMock;

        private readonly SubscriptionOrchestrationInvoker<Foo> _subscriptionOrchestrationInvoker;

        /// <summary>
        /// Common setup.
        /// </summary>
        public SubscriptionOrchestrationInvokerTests()
        {
            _subOrchestrationServiceMock = Moq.Create<ISubOrchestrationService<Foo>>();
            _serviceProviderMock = Moq.Create<IServiceProvider>();
            _serviceScopeFactoryMock = Moq.Create<IServiceScopeFactory>();
            _serviceScopeMock = Moq.Create<IServiceScope>();
            _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(_serviceScopeFactoryMock.Object);
            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);

            _subscriptionOrchestrationInvoker = new SubscriptionOrchestrationInvoker<Foo>(
                _serviceProviderMock.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SubscriptionOrchestrationInvoker{TMessage}.NextAsync"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task NextAsync()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            var fakePubSubMessage = FakeData.Create<PubsubMessage>();
            fakeContext.Items[ContextKeys.SubscriptionContextKey] = fakePubSubMessage;
            var serviceProviderMock = Moq.Create<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISubOrchestrationService<Foo>)))
                .Returns(_subOrchestrationServiceMock.Object);
            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            _serviceScopeMock.Setup(x => x.Dispose());
            _subOrchestrationServiceMock.Setup(
                    x => x.ExecuteAsync(
                        fakePubSubMessage.ToSubscriptionMessage<Foo>(),
                        fakeContext.RequestCancelledToken))
                .Returns(Task.CompletedTask);

            // act + assert
            await _subscriptionOrchestrationInvoker.NextAsync(fakeContext, null);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SubscriptionOrchestrationInvoker{TMessage}.NextAsync"/> when an exception is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task NextAsyncExceptionThrown()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            var fakePubSubMessage = FakeData.Create<PubsubMessage>();
            fakeContext.Items[ContextKeys.SubscriptionContextKey] = fakePubSubMessage;
            var serviceProviderMock = Moq.Create<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISubOrchestrationService<Foo>)))
                .Returns(_subOrchestrationServiceMock.Object);
            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            _serviceScopeMock.Setup(x => x.Dispose());
            _subOrchestrationServiceMock.Setup(
                    x => x.ExecuteAsync(
                        fakePubSubMessage.ToSubscriptionMessage<Foo>(),
                        fakeContext.RequestCancelledToken))
                .ThrowsAsync(new Exception());

            // act
            var actual = new Func<Task>(async () => await _subscriptionOrchestrationInvoker.NextAsync(fakeContext, null));

            // assert
            await actual.Should().ThrowExactlyAsync<Exception>();
        }
    }
}