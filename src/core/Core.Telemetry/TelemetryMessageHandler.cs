using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Telemetry.Http;

namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// A delegating handler to track telemetry of outbound http requests.
    /// </summary>
    public class TelemetryMessageHandler : DelegatingHandler
    {
        private readonly ITelemetryFactory _telemetryFactory;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="telemetryFactory">Emits diagnostic telemetry.</param>
        public TelemetryMessageHandler(ITelemetryFactory telemetryFactory)
        {
            _telemetryFactory = telemetryFactory;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var httpResponse = await base.SendAsync(request, cancellationToken);
            stopWatch.Stop();

            TrackTelemetry(request, httpResponse, DateTimeOffset.UtcNow, stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();

            return httpResponse;
        }

        private void TrackTelemetry(
            HttpRequestMessage request,
            HttpResponseMessage? response,
            DateTimeOffset startTime,
            double duration)
        {
            var isSuccess = response != null && response.IsSuccessStatusCode;
            _telemetryFactory.TrackDependencyHttpTelemetry(
                startTime,
                duration,
                ((int?)response?.StatusCode)?.ToString()!,
                request?.Method.ToString()!,
                request?.RequestUri?.ToString()!,
                !isSuccess,
                isSuccess);
        }
    }
}