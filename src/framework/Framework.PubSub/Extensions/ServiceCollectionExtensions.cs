using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Framework.PubSub.Subscription;
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
            services
                .AddTransient<ISubOrchestrationService<TMessage>, TMessageHandler>();
            services.AddHostedService<PubSubHostedService<TMessage>>();
            services.AddTransient<ISubscriberClientFactory, SubscriberClientFactory>();
        }
    }
}