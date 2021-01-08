using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Core.Testing.Client;
using HumanaEdge.Webcore.Framework.Testing.Integration.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.Testing.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a named instance of <see cref="TestClientFactory" /> used for integration testing.
        /// </summary>
        /// <param name="services"> The service collection. </param>
        /// <param name="name"> The name of the test client. </param>
        /// <param name="configuration"> The app configuration. </param>
        /// <returns> The same service collection for fluent chaining. </returns>
        public static IServiceCollection AddTestClientFactory(
            this IServiceCollection services,
            string name,
            IConfiguration configuration)
        {
            services.AddTransient<ITestClientFactory, TestClientFactory>();
            return services.Configure<TestClientOptions>(
                name,
                configuration.GetSection(name));
        }
    }
}