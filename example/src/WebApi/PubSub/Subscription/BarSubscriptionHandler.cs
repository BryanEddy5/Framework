using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub;

namespace HumanaEdge.Webcore.Example.WebApi.PubSub.Subscription
{
    /// <summary>
    /// A service that subscribes to a topic and handles messages published to that topic and subsequent subscription.
    /// </summary>
    public class BarSubscriptionHandler : ISubOrchestrationService<BarContract>
    {
        private int state;

        /// <summary>
        /// The underlying base class will call the invoke message and pass the deserialized message in so that the service may handle and perform any business orchestration logic.
        /// </summary>
        /// <param name="message">The published message shape.</param>
        /// <param name="cancellationToken">A cancellation token that can be invoked if the Acknowledgement window is exceeded.</param>
        /// <returns>An awaitable task for asynchronous execution.</returns>
        public async Task ExecuteAsync(BarContract message, CancellationToken cancellationToken)
        {
            state++;
            await Task.CompletedTask;
        }
    }
}