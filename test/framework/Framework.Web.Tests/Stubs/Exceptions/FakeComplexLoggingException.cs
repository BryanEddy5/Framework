using System;
using HumanaEdge.Webcore.Core.Common.Exceptions;

namespace HumanaEdge.Webcore.Framework.Web.Tests.Stubs.Exceptions
{
    /// <summary>
    /// An exception containing a complex object to be logged.
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
            : base(ExceptionMessage, ExceptionLoggedMessage, CreatFoo())
        {
        }

        private static Foo CreatFoo()
        {
            return new Foo
            {
                Bar = "bar",
                Buz = new Foo.BuzzClass { Bar = "Bar" }
            };
        }
    }
}