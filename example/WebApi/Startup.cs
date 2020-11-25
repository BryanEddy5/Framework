using HumanaEdge.Webcore.Core.Encryption;
using HumanaEdge.Webcore.Example.WebApi.Secrets;
using HumanaEdge.Webcore.Framework.Encryption.Extensions;
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

        /// <inheritdoc />
        protected override IHttpClientBuilder ConfigureAppServices(IServiceCollection services)
        {
            services.AddKmsEncryption(Configuration.GetSection(nameof(EncryptionServiceOptions)));
            services.AddSecret<FooSecret, FooSecretsOptions>(Configuration.GetSection(nameof(FooSecretsOptions)));
            return services.AddHttpClient("client");
        }
    }
}