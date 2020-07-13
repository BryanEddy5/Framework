using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Extensions
{
    /// <summary>
    ///     Contains extension methods for <see cref="IHostBuilder" />.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        ///     Configures the use of application specific dependency injection.
        /// </summary>
        /// <param name="hostBuilder">The web host builder.</param>
        /// <returns>The same web host builder for fluent chaining.</returns>
        public static IHostBuilder UseDependencyInjection(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory(
                cb => cb.RegisterWebcoreAttributedComponents()));
        }
    }
}