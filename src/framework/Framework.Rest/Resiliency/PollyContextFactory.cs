using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Rest.Resiliency;
using HumanaEdge.Webcore.Core.Web.Resiliency;
using Microsoft.Extensions.Logging;
using Polly;

namespace HumanaEdge.Webcore.Framework.Rest.Resiliency
{
    /// <inheritdoc />
    public class PollyContextFactory : IPollyContextFactory
    {
        private readonly IAccessTokenCacheService _accessTokenCacheService;

        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="accessTokenCacheService">Access token storage.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public PollyContextFactory(IAccessTokenCacheService accessTokenCacheService, ILoggerFactory loggerFactory)
        {
            _accessTokenCacheService = accessTokenCacheService;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public Context Create() => new Context()
            .WithLogger(_loggerFactory)
            .WithAccessTokenCache(_accessTokenCacheService);
    }
}