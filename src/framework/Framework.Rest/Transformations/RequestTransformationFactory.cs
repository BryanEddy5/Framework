using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Rest.Resiliency;
using HumanaEdge.Webcore.Core.Web.Resiliency;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;

namespace HumanaEdge.Webcore.Framework.Rest.Transformations
{
    /// <inheritdoc />
    public class RequestTransformationFactory : IRequestTransformationFactory
    {
        private readonly IAccessTokenCacheService _accessTokenCacheService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMediaTypeFormatter[] _mediaTypeFormatters;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="accessTokenCacheService">A cache for tokens.</param>
        /// <param name="httpContextAccessor">Provides access to http context.</param>
        /// <param name="mediaTypeFormatters">A collection of media type formatters.</param>
        public RequestTransformationFactory(
            IAccessTokenCacheService accessTokenCacheService,
            IHttpContextAccessor httpContextAccessor,
            IEnumerable<IMediaTypeFormatter> mediaTypeFormatters)
        {
            _accessTokenCacheService = accessTokenCacheService;
            _httpContextAccessor = httpContextAccessor;
            _mediaTypeFormatters = mediaTypeFormatters.ToArray();
        }

        /// <inheritdoc />
        public IRequestTransformationService Create(RestClientOptions options) =>
            new RequestTransformationService(_accessTokenCacheService, _httpContextAccessor, _mediaTypeFormatters, options);
    }
}