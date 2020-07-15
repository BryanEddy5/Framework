using System;
using System.Net;
using HumanaEdge.Webcore.Core.Web;

namespace HumanaEdge.Webcore.Framework.Web.Tests.Stubs.Exceptions
{
    /// <summary>
    /// A stubbed out exception that has a <see cref="HttpStatusCode.Forbidden" /> status code.
    /// </summary>
    [Serializable]
    public class FakeForbiddenMessageException : MessageAppException
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        public FakeForbiddenMessageException()
            : base("Forbidden action.")
        {
        }

        /// <inheritdoc />
        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
    }
}