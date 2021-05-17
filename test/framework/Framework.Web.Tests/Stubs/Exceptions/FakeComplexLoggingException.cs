using System;
using System.Net;
using HumanaEdge.Webcore.Core.Common;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Web;

namespace HumanaEdge.Webcore.Framework.Web.Tests.Stubs.Exceptions
{
    /// <summary>
    /// A stubbed out exception that has a <see cref="HttpStatusCode.NotFound" /> status code.
    /// </summary>
    [Serializable]
    internal sealed class FakeComplexLoggingException : MessageAppException
    {
        /// <summary>
        /// The fake message associated with the exception.
        /// </summary>
        internal const string ExceptionMessage = "Not Found.";

        /// <summary>
        /// The fake logged message that should not be returned back to the consumer.
        /// </summary>
        internal const string ExceptionLoggedMessage = "Logged messaged output";

        /// <summary>
        /// Designated ctor.
        /// </summary>
        public FakeComplexLoggingException()
            : base(ExceptionMessage, ExceptionLoggedMessage, CreateObject())
        {
        }

        private static Foo CreateObject()
        {
            return new Foo
            {
                Bar = "bar",
                Buz = new Foo.BuzzClass { Bar = "Bar" }
            };
        }
    }
}