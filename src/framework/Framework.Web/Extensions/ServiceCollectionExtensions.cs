using HumanaEdge.Webcore.Framework.Web.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.Web.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection" /> for registering services.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the services for the <see cref="IOptions{TOptions}" /> for accessing configuration settings.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The app configuration settings.</param>
        internal static void AddOptionsPattern(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ExceptionHandlingOptions>(configuration.GetSection(nameof(ExceptionHandlingOptions)));
        }
    }
}