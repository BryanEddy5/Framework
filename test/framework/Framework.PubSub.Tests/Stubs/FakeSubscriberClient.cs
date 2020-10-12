using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs
{
    /// <summary>
    /// A fake subscriber client to allow for unit testing.
    /// </summary>
    public class FakeSubscriberClient : SubscriberClient
    {
        /// <summary>
        /// The message we'd like to test.
        /// </summary>
        public PubsubMessage TestMessage { get; set; }

        /// <summary>
        /// The output of the method delegate.
        /// </summary>
        public Reply TestReply { get; set; }

        /// <summary>
        /// Overriding the StartAsync in order to immediately execute the given delegate on test data.
        /// </summary>
        /// <param name="handlerAsync">The method delegate.</param>
        /// <returns>A task.</returns>
        public override async Task StartAsync(Func<PubsubMessage, CancellationToken, Task<Reply>> handlerAsync)
        {
            TestReply = await handlerAsync.Invoke(TestMessage, CancellationToken.None);
        }
    }
}