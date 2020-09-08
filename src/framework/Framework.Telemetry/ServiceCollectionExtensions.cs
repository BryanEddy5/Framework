using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Core.Common;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Framework.Telemetry.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HumanaEdge.Webcore.Framework.Telemetry
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection" /> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds telemetry to the application.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddApplicationTelemetry(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITelemetrySink, LoggerSink>());
            services.AddSingleton<ITelemetryFactory, TelemetryFactory>();
        }
    }
}