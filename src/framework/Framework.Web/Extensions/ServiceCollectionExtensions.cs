using HumanaEdge.Webcore.Framework.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.Web.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection" /> for registering services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the services for the <see cref="IOptions{TOptions}" /> for accessing configuration settings.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The app configuration settings.</param>
        public static void AddOptionsPattern(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ExceptionHandlingOptions>(configuration.GetSection(nameof(ExceptionHandlingOptions)));
        }

        /// <summary>
        /// Registers the <see cref="IApiVersionDescriptionProvider"/>.
        /// </summary>
        /// <remarks>
        /// This does NOT enable versioning for the API! This only adds the dependency for the swagger UI explorer
        /// to know how to render any number (including one) of versions to the swagger UI. If you want to add
        /// API verisoning, use <see cref="EnableApiVersioning"/>.
        /// </remarks>
        /// <param name="services">The services collection.</param>
        public static void AddApiVersionDescriptionProvider(this IServiceCollection services)
        {
            services.AddTransient<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();
        }

        /// <summary>
        /// Registers the defaults for and enables Versioning in the API.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void EnableApiVersioning(this IServiceCollection services)
        {
            services.AddVersionedApiExplorer(
                setup =>
                {
                    setup.SubstituteApiVersionInUrl = true;
                    setup.GroupNameFormat = "'v'VVV";
                });
            services.AddApiVersioning(config =>
            {
                config.RegisterMiddleware = true;
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
                config.ReportApiVersions = true;
            });
        }
    }
}