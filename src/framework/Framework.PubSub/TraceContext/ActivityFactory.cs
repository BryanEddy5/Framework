using System;
using System.Diagnostics;
using Google.Cloud.PubSub.V1;
using Serilog.Context;

namespace HumanaEdge.Webcore.Framework.PubSub.TraceContext
{
    /// <inheritdoc />
    internal sealed class ActivityFactory : IActivityFactory
    {
        /// <summary>
        /// The trace id Span Length must be exactly equal to 32 or an exception will be thrown by .Net.
        /// We want to gracefully fail and avoid an exception being thrown if a trace id that doesn't conform to
        /// the W3C tract context is passed.
        /// </summary>
        private const int TraceIdLength = 32;

        /// <summary>
        /// The span id Span Length must be exactly equal to 16 or an exception will be thrown by .Net.
        /// We want to gracefully fail and avoid an exception being thrown if a span id that doesn't conform to
        /// the W3C tract context is passed.
        /// </summary>
        private const int SpanIdLength = 16;

        /// <inheritdoc/>
        public Activity Create(PubsubMessage message)
        {
            var activity =
                new Activity("HumanaEdge.PubSub.Subscription").AddBaggage(
                    TracingKeys.MessageId,
                    message.MessageId);
            if (message.Attributes.TryGetValue(TracingKeys.SpanId, out var spanId))
            {
                if (message.Attributes.TryGetValue(TracingKeys.TraceId, out var traceId))
                {
                    var activityTraceId = new Span<char>(traceId.ToCharArray());
                    var activitySpanId = new Span<char>(spanId.ToCharArray());
                    if (activityTraceId.Length == TraceIdLength && activitySpanId.Length == SpanIdLength)
                    {
                        activity.SetParentId(
                            ActivityTraceId.CreateFromString(activityTraceId),
                            ActivitySpanId.CreateFromString(activitySpanId),
                            ActivityTraceFlags.Recorded);
                    }
                }
            }

            activity.SetIdFormat(ActivityIdFormat.W3C).Start();

            LogContext.PushProperty(TracingKeys.MessageId, message.MessageId);
            LogContext.PushProperty(TracingKeys.TraceId, activity.RootId);
            LogContext.PushProperty(TracingKeys.ParentId, activity.ParentSpanId.ToString());
            LogContext.PushProperty(TracingKeys.SpanId, activity.SpanId.ToString());
            LogContext.PushProperty(TracingKeys.RequestId, message.MessageId);

            return activity;
        }
    }
}