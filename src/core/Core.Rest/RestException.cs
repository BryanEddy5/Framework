using System;
using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Core.Common;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    ///     An exception resulting from an attempted HTTP REST call.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class RestException : AppException
    {
        /// <summary>
        ///     Designated ctor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public RestException(string message, Exception? inner = null)
            : base(message, inner)
        {
            // nop
        }
    }
}