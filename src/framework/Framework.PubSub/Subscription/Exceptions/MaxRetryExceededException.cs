using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions
{
    /// <summary>
    /// Exception for when the max amount of retries has been exceeded.
    /// </summary>
    public class MaxRetryExceededException : PubSubException
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="maxNumberOfRetries">The max number of retries.</param>
        public MaxRetryExceededException(int maxNumberOfRetries)
        : base($"The max number of retires of {maxNumberOfRetries} has been exceeded.")
        {
        }

        /// <inheritdoc />
        public override Reply Reply => Reply.Ack;
    }
}