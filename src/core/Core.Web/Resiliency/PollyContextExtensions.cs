using Microsoft.Extensions.Logging;
using Polly;

namespace HumanaEdge.Webcore.Core.Web.Resiliency
{
    /// <summary>
    /// Extension methods for polly <see cref="Context"/>.
    /// </summary>
    public static class PollyContextExtensions
    {
        /// <summary>
        /// The unique key for extracting loggers out of the polly context.
        /// </summary>
        private const string LoggerKey = "LoggerKey";

        /// <summary>
        /// Sets a logger factory allowing consumers to access a logger without injecting it.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <param name="loggerFactory">A logger factory for generating a logger instance.</param>
        /// <returns>The same context for fluent chaining.</returns>
        public static Context WithLogger(this Context context, ILoggerFactory loggerFactory)
        {
            context[LoggerKey] = loggerFactory;
            return context;
        }

        /// <summary>
        /// Retrieves a logger factory allowing consumers to access a logger without injecting it.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <typeparam name="T">The source context for logging.</typeparam>
        /// <returns>A logger.</returns>
        public static ILogger<T> GetLogger<T>(this Context context)
        {
            if (context.TryGetValue(LoggerKey, out var loggerFactory))
            {
                var typeLoggerFactory = loggerFactory as ILoggerFactory;
                return typeLoggerFactory.CreateLogger<T>();
            }

            return null!;
        }

        /// <summary>
        /// Retrieves a logger factory allowing consumers to access a logger without injecting it.
        /// </summary>
        /// <param name="context">Context that carries with a single execution through a Policy.</param>
        /// <param name="loggerCategory">The category name for logging.</param>
        /// <returns>A logger.</returns>
        public static ILogger GetLogger(this Context context, string loggerCategory)
        {
            if (context.TryGetValue(LoggerKey, out var loggerFactory))
            {
                var typeLoggerFactory = loggerFactory as ILoggerFactory;
                return typeLoggerFactory!.CreateLogger(loggerCategory);
            }

            return null!;
        }
    }
}