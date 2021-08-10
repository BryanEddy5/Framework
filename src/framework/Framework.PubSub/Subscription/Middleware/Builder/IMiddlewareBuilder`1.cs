using HumanaEdge.Webcore.Core.PubSub.Subscription;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware.Builder
{
    /// <summary>
    /// Creates a pipeline instrumented with middleware.
    /// </summary>
    /// <typeparam name="TMessage">The message shape.</typeparam>
    internal interface IMiddlewareBuilder<TMessage>
    {
        /// <summary>
        /// Generates the middleware pipeline.
        /// </summary>
        /// <returns>The message delegate for pipeline invocation.</returns>
        MessageDelegate Build();
    }
}