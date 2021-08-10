using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Context
{
    /// <summary>
    /// Keys used for storing additional context.
    /// </summary>
    internal sealed class ContextKeys
    {
        /// <summary>
        /// Used to store the underlying <see cref="PubsubMessage"/>.
        /// </summary>
        public const string SubscriptionContextKey = "APP_MESSAGE_CONTEXT";
    }
}