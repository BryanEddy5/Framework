using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Framework.PubSub.Publication;
using HumanaEdge.Webcore.Framework.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.TraceContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.PubSub.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection" /> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add pub sub services to the application.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddPubSub(this IServiceCollection services)
        {
            services.AddTransient<ISubscriberClientFactory, SubscriberClientFactory>();
        }

        /// <summary>
        /// Add a hosted service for subscribing to a GCP subscription.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The app configuration settings.</param>
        /// <typeparam name="TMessage">The deserialized message structure.</typeparam>
        /// <typeparam name="TMessageHandler">The service that will consume the message.</typeparam>
        public static void AddSubscriptionHostedService<TMessage, TMessageHandler>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TMessageHandler : class, ISubOrchestrationService<TMessage>
        {
            services.AddOptions();
            services.Configure<PubSubOptions>(configuration.GetSection(nameof(PubSubOptions)));
            services.AddSingleton<ISubOrchestrationService<TMessage>, TMessageHandler>();
            services.AddHostedService<PubSubHostedService<TMessage>>();
            services.AddSingleton<ISubscriberClientFactory, SubscriberClientFactory>();
            services.AddSingleton<IActivityFactory, ActivityFactory>();
        }

        /// <summary>
        /// Registers a publisher client for publishing messages to a topic.
        /// </summary>
        /// <typeparam name="TMessage">The published message shape.</typeparam>
        /// <typeparam name="TOptions">The configuration options for setting up the publisher client.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="configurationSection">The configuration section key for the <see cref="PublisherOptions"/>.</param>
        public static void AddPublisherClient<TMessage, TOptions>(this IServiceCollection services, IConfigurationSection configurationSection)
        where TMessage : class
        where TOptions : PublisherOptions
        {
            services.AddOptions();
            services.Configure<TOptions>(configurationSection);
            services.AddSingleton<IPublisherClient<TMessage>, PublisherClient<TMessage>>();
            services.AddSingleton<IPublisherClientFactory, PublisherClientFactory>();
            services.AddSingleton<IPublishRequestConverter, PublishPublishRequestConverter>();
        }
    }
}