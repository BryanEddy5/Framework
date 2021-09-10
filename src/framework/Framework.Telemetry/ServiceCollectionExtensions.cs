using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Framework.Telemetry.Sinks;
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
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITelemetrySink, AlertSink>());
            services.AddSingleton<ITelemetryFactory, TelemetryFactory>();
        }

        /// <summary>
        /// Add's a message delegate handler to capture http dependency telemetry.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="name">The named client moniker.</param>
        /// <returns>The services for fluent chaining.</returns>
        public static IServiceCollection AddTelemetryHandler(this IServiceCollection services, string name)
        {
            services.AddApplicationTelemetry();
            services.AddTransient<TelemetryMessageHandler>();
            services.AddHttpClient(name).AddHttpMessageHandler<TelemetryMessageHandler>();
            return services;
        }
    }
}