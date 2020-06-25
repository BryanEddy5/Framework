using CorrelationId.Abstractions;
using HumanaEdge.Webcore.Core.Web;

namespace HumanaEdge.Webcore.Framework.Web.Request
{
    /// <inheritdoc />
    internal sealed class RequestIdAccessor : IRequestIdAccessor
    {
        /// <summary>
        ///     An accessor for the request context.
        /// </summary>
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequestIdAccessor" /> class.
        /// </summary>
        /// <param name="correlationContextAccessor">An accessor for the request context.</param>
        public RequestIdAccessor(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        /// <inheritdoc />
        public string Get => _correlationContextAccessor.CorrelationContext.CorrelationId;
    }
}