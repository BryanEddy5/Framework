using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Telemetry.PubSub;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware
{
    /// <summary>
    /// Captures telemetry and diagnostic information for a subscription.
    /// </summary>
    /// <typeparam name="TMessage">The message shape.</typeparam>
    internal sealed class RequestInfoMiddleware<TMessage> : ISubscriptionMiddleware<TMessage>
    {
        private readonly ITelemetryFactory _telemetryFactory;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="telemetryFactory">Captures telemetry.</param>
        public RequestInfoMiddleware(ITelemetryFactory telemetryFactory)
        {
            _telemetryFactory = telemetryFactory;
        }

        /// <inheritdoc />
        public async Task NextAsync(
            ISubscriptionContext subscriptionMessage,
            MessageDelegate messageDelegate)
        {
            var success = false;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await messageDelegate.Invoke(subscriptionMessage);
                success = true;
            }
            finally
            {
                stopwatch.Stop();
                var duration = stopwatch.ElapsedMilliseconds;
                TrackTelemetry(success, duration, subscriptionMessage);
            }
        }

        /// <summary>
        /// Short-hand method for tracking telemetry in this Middleware.
        /// </summary>
        /// <param name="success">Designates if the request was successful.</param>
        /// <param name="duration">The total duration of the request.</param>
        /// <param name="message">The pubsub message.</param>
        private void TrackTelemetry(bool success, double duration, ISubscriptionContext message)
        {
            _telemetryFactory.TrackSubscriptionTelemetry(
                DateTimeOffset.UtcNow,
                message.MessageId,
                duration,
                success);
        }
    }
}