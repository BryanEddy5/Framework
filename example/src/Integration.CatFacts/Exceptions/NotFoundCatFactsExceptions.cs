using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using HumanaEdge.Webcore.Core.Common.Exceptions;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Exceptions
{
    /// <summary>
    /// An Cat Fact specific exception.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class NotFoundCatFactsExceptions : MessageAppException
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="relativePath">The relative path of the Uri for the request.</param>
        /// <param name="exception">An optional inner exception to be wrapped.</param>
        public NotFoundCatFactsExceptions(string relativePath, Exception? exception = null)
            : base($"No cat facts could be found for the relative path of: {relativePath}", exception)
        {
        }

        /// <inheritdoc />
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}