using CorrelationId;
using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;

namespace HumanaEdge.Webcore.Framework.Web.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder" />.
    /// </summary>
    internal static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the use of health of all registered checks with the 'ready' tag.
        /// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2.
        /// </summary>
        /// <param name="app">The application builder.</param>
        internal static void UseReadyHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks(
                "/health/ready",
                new HealthCheckOptions
                {
                    Predicate = check => false // check.Tags.Contains("ready"),
                });

            app.UseHealthChecks(
                "/health/live",
                new HealthCheckOptions
                {
                    Predicate = _ => false // Exclude all checks and return a 200-Ok.
                });
        }

        /// <summary>
        /// Configures the use of middleware to generate the request id.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>Returns the app builder for fluent chaining.</returns>
        internal static IApplicationBuilder UseRequestMiddleware(this IApplicationBuilder app)
        {
            return app.UseCorrelationId();
        }

        /// <summary>
        /// Configures the application to use tracing.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="configuration">The application configuration settings.</param>
        internal static void UseTracing(this IApplicationBuilder app, IConfiguration configuration)
        {
            // configure Google Stackdriver Trace only if we have the configuration section defined
            // https://cloud.google.com/trace/
            if (configuration.GetValue<bool>(ConfigurationProperties.StackdriverEnabledKey))
            {
                app.UseGoogleTrace();
            }
        }
    }
}