using HumanaEdge.Webcore.Core.Soap.Client.Factory;
using HumanaEdge.Webcore.Framework.Soap.Factory;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.Soap.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the necessary services for SOAPey client support.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The same service collection, for fluently chaining.</returns>
        public static IServiceCollection AddSoapClient(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IPollyContextFactory, PollyContextFactory>()
                .AddSingleton<IEndpointBehaviorFactory, EndpointBehaviorFactory>();
        }
    }
}