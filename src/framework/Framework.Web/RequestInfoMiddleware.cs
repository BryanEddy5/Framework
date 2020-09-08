using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Telemetry.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HumanaEdge.Webcore.Framework.Web
{
    /// <summary>
    /// Middleware which establish a common set of items in the logical log context.
    /// </summary>
    internal sealed class RequestInfoMiddleware
    {
        /// <summary>
        /// Delegate to invoke the next handler in the pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The manager for tracking telemetry.
        /// </summary>
        private readonly ITelemetryFactory _telemetryFactory;

        /// <summary>
        /// Designated constructor.
        /// </summary>
        /// <param name="next">A delegate to invoke the next handler in the pipeline.</param>
        /// <param name="telemetryFactory">The telemetry manager.</param>
        public RequestInfoMiddleware(
            RequestDelegate next,
            ITelemetryFactory telemetryFactory)
        {
            _next = next;
            _telemetryFactory = telemetryFactory;
        }

        /// <summary>
        /// Handler for HTTP requests which establishes a standard set of items into the log context.
        /// Also handles telemetry emitting.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>An awaitable task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                await _next(context);
            }
            finally
            {
                stopWatch.Stop();
                TrackTelemetry(context, stopWatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Method to decipher if the request was successful by interpreting the status code.
        /// </summary>
        /// <param name="statusCode">An http status code.</param>
        /// <returns>boolean flag of success.</returns>
        private bool IsSuccessfulRequest(int statusCode) => statusCode >= 200 && statusCode <= 299;

        /// <summary>
        /// Short-hand method for tracking telemetry in this Middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /> used for telemetry.</param>
        /// <param name="duration">The total duration of the request.</param>
        private void TrackTelemetry(HttpContext context, double duration)
        {
            _telemetryFactory.TrackRequestHttpTelemetry(
                startTime: DateTimeOffset.UtcNow,
                duration: duration,
                responseCode: context.Response.StatusCode.ToString(),
                httpMethod: context.Request.Method,
                uri: context.Request.GetDisplayUrl(),
                success: IsSuccessfulRequest(context.Response.StatusCode));
        }
    }
}