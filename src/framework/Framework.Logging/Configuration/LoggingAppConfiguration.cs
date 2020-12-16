using System;
using System.Diagnostics.CodeAnalysis;
using Destructurama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Serilog;
using Serilog.Debugging;
using Serilog.Exceptions;

namespace HumanaEdge.Webcore.Framework.Logging.Configuration
{
    /// <summary>
    /// Dynamically configures logging based on the application configuration settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class LoggingAppConfiguration
    {
        /// <summary>
        /// Allows for the logger to produce diagnostics for detecting issues with logging.
        /// </summary>
        internal const string UseLoggingDiagnostics = "Logging:UseLoggingDiagnostics";

        /// <summary>
        /// Alters the targets for nLog based on the appsettings.{local.}json configuration file.
        /// </summary>
        /// <param name="configuration"> The applications configuration file. <see cref="IConfiguration" />.</param>
        /// <typeparam name="TEntry">A type from the entry point assembly.</typeparam>
        public static void Configure<TEntry>(IConfiguration configuration)
        {
            if (configuration.GetValue(UseLoggingDiagnostics, false))
            {
                SelfLog.Enable(Console.WriteLine);
            }

            var dependencyContext = DependencyContext.Load(typeof(TEntry).Assembly);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration, dependencyContext)
                .WriteTo.Debug()
                .Destructure.UsingAttributes()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .CreateLogger();
        }
    }
}