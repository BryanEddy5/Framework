using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace HumanaEdge.Webcore.Core.Common.Exceptions
{
    /// <summary>
    /// An application specific exception for transmitting errors back to the consumer.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class AppException : Exception
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">Optional. Inner exception, if any.</param>
        protected AppException(string message, Exception? inner = null)
            : base(message, inner)
        {
            // nop
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // nop
        }
    }
}