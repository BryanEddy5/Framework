using CorrelationId.DependencyInjection;
using HumanaEdge.Webcore.Core.Web;
using HumanaEdge.Webcore.Framework.Web.Options;
using HumanaEdge.Webcore.Framework.Web.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.Web.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="IServiceCollection" /> for registering services.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers the request id accessor in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The services collection.</param>
        internal static void AddRequestIdAccessor(this IServiceCollection services)
        {
            services.AddDefaultCorrelationId();
            services.AddSingleton<IRequestIdAccessor, RequestIdAccessor>();
        }

        /// <summary>
        ///     Registers the services for the <see cref="IOptions{TOptions}" /> for accessing configuration settings.
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