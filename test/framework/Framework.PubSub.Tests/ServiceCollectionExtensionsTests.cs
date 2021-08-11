using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Extensions;
using HumanaEdge.Webcore.Framework.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware;
using HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs;
using HumanaEdge.Webcore.Framework.PubSub.TraceContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="ServiceCollectionExtensions" /> class.
    /// </summary>
    public class ServiceCollectionExtensionsTests : BaseTests
    {
        private static readonly string _messageKey = nameof(PubSubOptions)!;

        /// <summary>
        /// Verifies the behavior of the
        /// <see cref="ServiceCollectionExtensions.AddSubscriptionHostedService{TMessage, TMessageHandler}(IServiceCollection, IConfigurationSection, Type[])" />.
        /// </summary>
        [Fact]
        public void AddSubscriptionHostedServiceTest()
        {
            // arrange
            var serviceProvider = CreateServiceProvider();

            // act
            var activityFactory = serviceProvider.GetRequiredService<IActivityFactory>();
            var fooOrchestrationService = serviceProvider.GetRequiredService<ISubOrchestrationService<Foo>>();
            var pubSubHostedService = serviceProvider.GetRequiredService<IHostedService>();

            // assert
            Assert.IsType<ActivityFactory>(activityFactory);
            Assert.IsType<FooSubscriptionHandler>(fooOrchestrationService);
            Assert.IsType<SubscriberHostedService<Foo>>(pubSubHostedService);
        }

        /// <summary>
        /// Verifies the behavior of the
        /// <see cref="ServiceCollectionExtensions.AddSubscriptionHostedService{TMessage, TMessageHandler}(IServiceCollection, IConfigurationSection, Type[])" />.
        /// that the middleware is a returned in the correct order.
        /// </summary>
        [Fact]
        public void AddMiddleware()
        {
            // arrange
            var types = new[] { typeof(StubMiddleware<Foo>) };
            var serviceProvider = CreateServiceProvider(types);

            // act
            var actual = serviceProvider.GetRequiredService<IEnumerable<ISubscriptionMiddleware<Foo>>>().ToArray();

            // assert
            actual[0].Should().BeOfType<RequestInfoMiddleware<Foo>>();
            actual[1].Should().BeOfType<ExceptionHandlingMiddleware<Foo>>();
            actual[2].Should().BeOfType<MaxRetryMiddleware<Foo>>();
            actual[3].Should().BeOfType<StubMiddleware<Foo>>();
            actual.Last().Should().BeOfType<SubscriptionOrchestrationInvoker<Foo>>();
        }

        /// <summary>
        /// Verifies the behavior of the
        /// <see cref="ServiceCollectionExtensions.AddSubscriptionHostedService{TMessage, TMessageHandler}(IServiceCollection, IConfigurationSection, Type[])" />.
        /// when a middleware isn't properly implementing the generic argument.
        /// </summary>
        [Fact]
        public void AddIncorrectMiddleware()
        {
            // arrange
            var types = new[] { typeof(WrongMiddleware) };

            // act
            var actual = new Func<IServiceProvider>(() => CreateServiceProvider(types));

            // assert
            actual.Should().ThrowExactly<InvalidSubscriptionMiddlewareException>();
        }

        private IConfiguration ConfigurationBuilder()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { _messageKey, "buz" },
                { _messageKey + ":ProjectId", "foo" },
                { _messageKey + ":Name", "bar" },
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        private IServiceProvider CreateServiceProvider(Type[] middlewares = null)
        {
            var services =
                new ServiceCollection();
            services.AddSubscriptionHostedService<Foo, FooSubscriptionHandler>(ConfigurationBuilder(), middlewares);
            services.AddLogging();
            var telemetryFactory = new Mock<ITelemetryFactory>().Object;
            services.AddSingleton(telemetryFactory);
            return services.BuildServiceProvider();
        }

        private class StubMiddleware<TMessage> : ISubscriptionMiddleware<TMessage>
        {
            public Task NextAsync(ISubscriptionContext subscriptionMessage, MessageDelegate messageDelegate)
            {
                throw new NotImplementedException();
            }
        }

        private class WrongMiddleware : ISubscriptionMiddleware<Foo>
        {
            public Task NextAsync(ISubscriptionContext subscriptionMessage, MessageDelegate messageDelegate)
            {
                throw new NotImplementedException();
            }
        }
    }
}