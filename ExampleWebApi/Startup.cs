using HumanaEdge.Webcore.Framework.Encryption.Extensions;
using HumanaEdge.Webcore.Framework.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.ExampleWebApi
{
    /// <summary>
    /// ExampleWebApi's Startup class.
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
            services.AddKmsEncryption(Configuration.GetSection("EncryptionServiceOptions"));
            return services.AddHttpClient("client");
        }
    }
}