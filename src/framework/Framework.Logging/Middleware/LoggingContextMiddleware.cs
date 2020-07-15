using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Web;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace HumanaEdge.Webcore.Framework.Logging.Middleware
{
    /// <summary>
    /// Middleware which establish a common set of items in the logical log context.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class LoggingContextMiddleware
    {
        /// <summary>
        /// Key used to store the correlation id in the <see cref="IDiagnosticContext" />.
        /// </summary>
        internal const string CorrelationIdKey = "CorrelationId";

        /// <summary>
        /// The log context, used to establish log properties.
        /// </summary>
        private readonly IDiagnosticContext _diagnosticContext;

        /// <summary>
        /// Delegate to invoke the next handler in the pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The request id accessor.
        /// </summary>
        private readonly IRequestIdAccessor _requestIdAccessor;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="next">A delegate to invoke the next handler in the pipeline.</param>
        /// <param name="diagnosticContext">The log context.</param>
        /// <param name="requestIdAccessor">The request id accessor.</param>
        public LoggingContextMiddleware(
            RequestDelegate next,
            IDiagnosticContext diagnosticContext,
            IRequestIdAccessor requestIdAccessor)
        {
            _next = next;
            _diagnosticContext = diagnosticContext;
            _requestIdAccessor = requestIdAccessor;
        }

        /// <summary>
        /// Handler for HTTP requests which establishes a standard set of items into the log context.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>An awaitable task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            _diagnosticContext.Set(CorrelationIdKey, _requestIdAccessor.CorrelationId);

            await _next(context);
        }
    }
}