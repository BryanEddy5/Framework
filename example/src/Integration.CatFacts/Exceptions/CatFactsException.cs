using System;
using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Core.Common.Exceptions;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Exceptions
{
    /// <summary>
    /// An Cat Fact specific exception.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class CatFactsException : MessageAppException
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="message">A read friendly message associated with the exception.</param>
        /// <param name="exception">An optional inner exception to be wrapped.</param>
        public CatFactsException(string message, Exception? exception = null)
            : base(message, exception)
        {
        }
    }
}