using System;

namespace HumanaEdge.Webcore.Core.PubSub.Exceptions
{
    /// <summary>
    /// An exception thrown if the middleware doesn't contain the correct generic argument.
    /// </summary>
    public class InvalidSubscriptionMiddlewareException : Exception
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidSubscriptionMiddlewareException(string message)
            : base(message)
        {
        }
    }
}