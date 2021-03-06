using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace HumanaEdge.Webcore.Core.Common.Exceptions
{
    /// <summary>
    /// A class containing extension methods for <see cref="Exception" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Throws an exception while preserving the stack trace.
        /// </summary>
        /// <param name="exception">The exception to be re-thrown.</param>
        public static void Rethrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}