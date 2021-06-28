using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Framework.Rest.Resiliency;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    internal sealed class RestClientFactory : IRestClientFactory
    {
        /// <summary>
        /// A factory for generating a rest client.
        /// </summary>
        private readonly IInternalClientFactory _internalClientFactory;

        private readonly IPollyContextFactory _pollyContextFactory;

        private readonly IAccessTokenCacheService _accessTokenCacheService;

        /// <summary>
        /// A collection of media types for formatting a request.
        /// </summary>
        private readonly IMediaTypeFormatter[] _mediaTypeFormatters;

        private readonly ITelemetryFactory _telemetryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientFactory" /> class.
        /// </summary>
        /// <param name="internalClientFactory">A factory for generating a rest client.</param>
        /// <param name="mediaTypeFormatters">A collection of media types for formatting a request.</param>
        /// <param name="pollyContextFactory">A factory for creating Polly Context.</param>
        /// <param name="accessTokenCacheService">A token caching service. </param>
        /// <param name="telemetryFactory">A factory associated with telemetry.</param>
        public RestClientFactory(
            IInternalClientFactory internalClientFactory,
            IEnumerable<IMediaTypeFormatter> mediaTypeFormatters,
            IPollyContextFactory pollyContextFactory,
            IAccessTokenCacheService accessTokenCacheService,
            ITelemetryFactory telemetryFactory = null!)
        {
            _internalClientFactory = internalClientFactory;
            _pollyContextFactory = pollyContextFactory;
            _accessTokenCacheService = accessTokenCacheService;
            _mediaTypeFormatters = mediaTypeFormatters.ToArray();
            _telemetryFactory = telemetryFactory;
        }

        /// <inheritdoc />
        public IRestClient CreateClient<TRestClient>(RestClientOptions options)
        {
            return new RestClient(
                typeof(TRestClient).Name,
                _internalClientFactory,
                options,
                _mediaTypeFormatters,
                _pollyContextFactory,
                _accessTokenCacheService,
                _telemetryFactory);
        }
    }
}