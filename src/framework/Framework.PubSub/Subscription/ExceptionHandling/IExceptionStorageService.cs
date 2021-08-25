using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling
{
    /// <summary>
    /// A service for uploading exceptions to a storage bucket.
    /// </summary>
    internal interface IExceptionStorageService
    {
        /// <summary>
        /// Loads the message to the bucket.
        /// </summary>
        /// <param name="payload">The message that failed to process.</param>
        /// <param name="exception">The exception message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TMessage">The payload shape.</typeparam>
        /// <returns>An awaitable task.</returns>
        Task LoadException<TMessage>(string payload, Exception exception, CancellationToken cancellationToken);
    }
}