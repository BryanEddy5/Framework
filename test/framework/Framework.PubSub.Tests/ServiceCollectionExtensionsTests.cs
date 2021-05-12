using System.Collections.Generic;
using AutoFixture;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Extensions;
using HumanaEdge.Webcore.Framework.PubSub.Subscription;
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
        /// <see cref="ServiceCollectionExtensions.AddSubscriptionHostedService{TMessage, TMessageHandler}(IServiceCollection, IConfigurationSection)" />.
        /// </summary>
        [Fact]
        public void AddSubscriptionHostedServiceTest()
        {
            // arrange
            var services =
                new ServiceCollection();
            services.AddSubscriptionHostedService<Foo, FooSubscriptionHandler>(ConfigurationBuilder());
            services.AddLogging();
            var telemetryFactory = new Mock<ITelemetryFactory>().Object;
            services.AddSingleton(telemetryFactory);
            var serviceProvider = services.BuildServiceProvider();

            // act
            var activityFactory = serviceProvider.GetRequiredService<IActivityFactory>();
            var fooOrchestrationService = serviceProvider.GetRequiredService<ISubOrchestrationService<Foo>>();
            var pubSubHostedService = serviceProvider.GetRequiredService<IHostedService>();

            // assert
            Assert.IsType<ActivityFactory>(activityFactory);
            Assert.IsType<FooSubscriptionHandler>(fooOrchestrationService);
            Assert.IsType<PubSubHostedService<Foo>>(pubSubHostedService);
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
    }
}