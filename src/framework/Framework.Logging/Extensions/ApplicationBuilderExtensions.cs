using HumanaEdge.Webcore.Framework.Logging.Middleware;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace HumanaEdge.Webcore.Framework.Logging.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="IApplicationBuilder" /> for logging.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     Registers logging context middleware with the <see cref="IApplicationBuilder" />.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>Returns the application builder for fluent chaining.</returns>
        public static IApplicationBuilder UseLoggingContextMiddleware(this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging()
                .UseMiddleware<LoggingContextMiddleware>();
        }
    }
}