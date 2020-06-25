using System.Net.Http;
using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Diagnostics.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.Web.Exceptions
{
    /// <summary>
    ///     Extension methods for the <see cref="IServiceCollection" /> class.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the google stackdriver trace service.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The app configuration settings.</param>
        /// <param name="httpClientBuilder">A builder for configuring named <see cref="HttpClient"/> instances.</param>
        internal static void AddTracing(
            this IServiceCollection services,
            IConfiguration configuration,
            IHttpClientBuilder httpClientBuilder)
        {
            if (configuration.GetValue<bool>(ConfigurationProperties.StackdriverEnabledKey))
            {
                services.AddGoogleTrace(
                    options =>
                    {
                        options.ProjectId =
                            configuration.GetValue<string>(ConfigurationProperties.StackdriverProjectIdKey);
                        options.Options = TraceOptions.Create(
                            bufferOptions: BufferOptions.NoBuffer());
                    });
                httpClientBuilder.AddOutgoingGoogleTraceHandler();
            }
        }
    }
}