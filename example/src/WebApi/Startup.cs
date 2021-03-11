using HumanaEdge.Webcore.Core.Encryption;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Example.WebApi.PubSub;
using HumanaEdge.Webcore.Example.WebApi.PubSub.Publication;
using HumanaEdge.Webcore.Example.WebApi.PubSub.Subscription;
using HumanaEdge.Webcore.Example.WebApi.Secrets;
using HumanaEdge.Webcore.Framework.Encryption.Extensions;
using HumanaEdge.Webcore.Framework.PubSub.Extensions;
using HumanaEdge.Webcore.Framework.SecretsManager.Extensions;
using HumanaEdge.Webcore.Framework.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Example.WebApi
{
    /// <summary>
    /// WebApi's Startup class.
    /// </summary>
    public class Startup : BaseStartup<Startup>
    {
        /// <summary>
        /// designated ctor.
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>>
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        /// <summary>
        /// Tailor service registration for a particular microservice.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for all services to be registered.</param>
        /// <returns>An named http client.</returns>
        protected override IHttpClientBuilder ConfigureAppServices(IServiceCollection services)
        {
            services.AddKmsEncryption(Configuration.GetSection(nameof(EncryptionServiceOptions))); // Register Encryption Service
            services.AddSecret<FooSecret, FooSecretsOptions>(Configuration.GetSection(nameof(FooSecretsOptions))); // Register Secrets Manager Service
            services.AddSubscriptionHostedService<FooContract, FooSubscriptionHandler>(Configuration.GetSection("FooSubscriptionOptions")); // Register Subscription Handler
            services.AddPublisherClient<FooContract>(Configuration.GetSection("FooPublisherOptions")); // Register Publisher Client
            return services.AddHttpClient("client");  // Pass back a named http client to be used for tracing
        }
    }
}