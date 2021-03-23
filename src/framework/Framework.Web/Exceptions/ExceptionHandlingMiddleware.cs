using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Common;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Web;
using HumanaEdge.Webcore.Framework.Web.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HumanaEdge.Webcore.Framework.Web.Exceptions
{
    /// <summary>
    /// A middleware to handle exceptions thrown.
    /// </summary>
    public sealed class ExceptionHandlingMiddleware
    {
        /// <summary>
        /// The default error response when an exception occurs.
        /// </summary>
        internal static readonly string DefaultErrorMessage =
            $"An error occured during the request.  See {nameof(ProblemDetail.Message)} for additional detail.";

        /// <summary>
        /// Settings for the JSON serialization.
        /// </summary>
        private readonly JsonSerializerSettings _jsonSettings;

        /// <summary>
        /// The application logger.
        /// </summary>
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// A delegate used to invoke the next step in the web pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The application configuration settings for the exception handling middleware.
        /// </summary>
        private readonly IOptionsMonitor<ExceptionHandlingOptions> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware" /> class.
        /// </summary>
        /// <param name="next">A delegate used to invoke the next step in the web pipeline.</param>
        /// <param name="logger">The application logger.</param>
        /// <param name="options">The application configuration settings.</param>
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IOptionsMonitor<ExceptionHandlingOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options;
            _jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>
        /// High level exception handler for exceptions thrown for requests sent down the pipeline.
        /// </summary>
        /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" />HttpContext delegate.</param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                _logger.LogInformation(exception, "An exception was thrown");
                await HandleExceptionAsync(exception, httpContext);
            }
        }

        /// <summary>
        /// Generates a <see cref="ProblemDetail" /> for the response with information about the exception thrown.
        /// </summary>
        /// <param name="exception">The exception that has been caught.</param>
        /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" />HttpContext delegate.</param>
        private ProblemDetail GenerateProblemDetailResponse(Exception exception, HttpContext httpContext)
        {
            var message = exception.Message;
            var statusCode = HttpStatusCode.InternalServerError;

            if (exception is AggregateException aggregateException)
            {
                var firstMessageAppException = aggregateException.InnerExceptions.FirstOrDefault(e => e is MessageAppException) as MessageAppException;
                message = firstMessageAppException?.Message ?? message;
                statusCode = firstMessageAppException?.StatusCode ?? statusCode;
            }

            if (exception is MessageAppException httpException)
            {
                message = httpException.Message;
                statusCode = httpException.StatusCode;
            }

            var traceId = Activity.Current?.Id;
            var response = new ProblemDetail(
                DefaultErrorMessage,
                httpContext.TraceIdentifier ?? Guid.NewGuid().ToString(),
                statusCode,
                message,
                traceId !);
            if (_options.CurrentValue.ShowExceptionDetails)
            {
                response = new DebugProblemDetail(
                    response,
                    exception);
            }

            return response;
        }

        /// <summary>
        /// Handles exceptions thrown.
        /// </summary>
        /// <param name="exception">The exception thrown from the application.</param>
        /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" />HttpContext delegate.</param>
        /// <returns>An awaitable task.</returns>
        private Task HandleExceptionAsync(Exception exception, HttpContext httpContext)
        {
            // We can't do anything if the response has already started, just abort.
            if (httpContext.Response.HasStarted)
            {
                _logger.LogError(
                    exception,
                    "The response has already started, the error handler will not be executed");
                exception.Rethrow();
            }

            var response = GenerateProblemDetailResponse(exception, httpContext);
            var result = JsonConvert.SerializeObject(response, _jsonSettings);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)response.Status;
            return httpContext.Response.WriteAsync(result);
        }
    }
}