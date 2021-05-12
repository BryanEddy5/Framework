using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs
{
    /// <summary>
    /// A stub for testing that implements the original method of <see cref="ISubOrchestrationService{TMessage}.ExecuteAsync(TMessage, CancellationToken)"/>.
    /// </summary>
    public class FooSubscriptionHandler : ISubOrchestrationService<Foo>
    {
        /// <inheritdoc />
        public Task ExecuteAsync(Foo message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}