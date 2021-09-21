using HumanaEdge.Webcore.Core.Web.Resiliency;
using Microsoft.Extensions.Logging;
using Polly;

namespace HumanaEdge.Webcore.Framework.Soap.Resiliency
{
    /// <inheritdoc />
    public sealed class PollyContextFactory : IPollyContextFactory
    {
        /// <inheritdoc cref="ILoggerFactory"/>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="loggerFactory">Logger factory.</param>
        public PollyContextFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public Context Create()
        {
            return new Context().WithLogger(_loggerFactory);
        }
    }
}