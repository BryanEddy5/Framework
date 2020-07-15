using System;
using System.Diagnostics.CodeAnalysis;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// An exception thrown when a given request body was not able to be formatted according to the declared
    /// <see cref="MediaType" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public sealed class FormatFailedRestException : RestException
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public FormatFailedRestException(string message, Exception? inner = null)
            : base(message, inner)
        {
            // nop
        }
    }
}