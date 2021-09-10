using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Web.Contracts;

namespace HumanaEdge.Webcore.Core.Web.Exceptions
{
    /// <summary>
    /// Thrown when a 400 http status response.
    /// </summary>
    public sealed class BadRequestException : MessageAppException
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="badRequestResponse">The bad exception response.</param>
        public BadRequestException(BadRequestResponse badRequestResponse)
            : base("The underlying service received an unexpected response.", "The request generated a 400 bad request {@Response}", badRequestResponse)
        {
            BadRequestResponse = badRequestResponse;
        }

        /// <summary>
        /// Contains the bad request context.
        /// </summary>
        public BadRequestResponse BadRequestResponse { get; }
    }
}