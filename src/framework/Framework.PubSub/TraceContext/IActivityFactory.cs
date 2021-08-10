using System.Diagnostics;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.TraceContext
{
    /// <summary>
    /// A factory pattern for creating a new <see cref="Activity"/> for W3C trace context.
    /// </summary>
    internal interface IActivityFactory
    {
        /// <summary>
        /// Creates a new activity for Pub/Sub subscriptions.
        /// </summary>
        /// <param name="message">The pub/sub message.</param>
        /// <returns>A new activity.</returns>
        Activity Create(PubsubMessage message);
    }
}