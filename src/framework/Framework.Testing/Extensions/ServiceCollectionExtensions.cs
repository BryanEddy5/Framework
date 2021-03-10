using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Core.Testing.Client;
using HumanaEdge.Webcore.Core.Testing.Integration;
using HumanaEdge.Webcore.Framework.Testing.Integration.Client;
using HumanaEdge.Webcore.Framework.Testing.Integration.Data;
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

        /// <summary>
        /// Registers <see cref="ITestData{TData}"/> for retrieving test data for integration testing.
        /// </summary>
        /// <param name="services"> The service collection. </param>
        /// <typeparam name="TData">The test data shape.</typeparam>
        /// <returns> The same service collection for fluent chaining. </returns>
        public static IServiceCollection AddTestData<TData>(this IServiceCollection services)
        where TData : class
        {
           return services.AddSingleton<ITestData<TData>, TestData<TData>>();
        }
    }
}