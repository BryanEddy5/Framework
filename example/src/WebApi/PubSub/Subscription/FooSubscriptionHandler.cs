using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Example.WebApi.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HumanaEdge.Webcore.Example.WebApi.PubSub.Subscription
{
    /// <summary>
    /// A service that subscribes to a topic and handles messages published to that topic and subsequent subscription.
    /// </summary>
    public class FooSubscriptionHandler : ISubOrchestrationService<FooContract>
    {
        private readonly IScopedService _scopedService;

        /// <summary>
        /// Services can be injected using DI just like any other class.
        /// </summary>
        /// <param name="scopedService"><see cref="IHostedService"/>s are singletons and thus any services injected will also become singletons even if registered as scoped or transient.</param>
        public FooSubscriptionHandler(IScopedService scopedService)
        {
            _scopedService = scopedService;
        }

        /// <summary>
        /// The underlying base class will call the invoke message and pass the deserialized message in so that the service may handle and perform any business orchestration logic.
        /// </summary>
        /// <param name="message">The published message shape.</param>
        /// <param name="cancellationToken">A cancellation token that can be invoked if the Acknowledgement window is exceeded.</param>
        /// <returns>An awaitable task for asynchronous execution.</returns>
        public async Task ExecuteAsync(FooContract message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            if (message.Name == "Some exceptional condition")
            {
                throw new UnrecoverableException();
            }
            else
            {
                throw new RecoverableException();
            }
        }
    }
}