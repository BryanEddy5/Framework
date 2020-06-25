using System;
using System.Net;
using HumanaEdge.Webcore.Core.Web;

namespace HumanaEdge.Webcore.Framework.Web.Tests.Stubs.Exceptions
{
    /// <summary>
    ///     A stubbed out exception that has a <see cref="HttpStatusCode.NotFound" /> status code.
    /// </summary>
    [Serializable]
    internal sealed class FakeNotFoundMessageException : MessageAppException
    {
        /// <summary>
        ///     The fake message associated with the exception.
        /// </summary>
        internal const string ExceptionMessage = "Not Found.";

        /// <summary>
        ///     Designated ctor.
        /// </summary>
        public FakeNotFoundMessageException()
            : base(ExceptionMessage)
        {
        }

        /// <inheritdoc />
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}