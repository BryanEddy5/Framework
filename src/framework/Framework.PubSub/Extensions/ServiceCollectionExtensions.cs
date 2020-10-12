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
    }
}