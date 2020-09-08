﻿using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Telemetry;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    internal sealed class RestClientFactory : IRestClientFactory
    {
        /// <summary>
        /// A factory for generating a rest client.
        /// </summary>
        private readonly IInternalClientFactory _internalClientFactory;

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
        /// /// <param name="telemetryFactory">A factory associated with telemetry.</param>
        public RestClientFactory(
            IInternalClientFactory internalClientFactory,
            IEnumerable<IMediaTypeFormatter> mediaTypeFormatters,
            ITelemetryFactory telemetryFactory = null!)
        {
            _internalClientFactory = internalClientFactory;
            _mediaTypeFormatters = mediaTypeFormatters.ToArray();
            _telemetryFactory = telemetryFactory;
        }

        /// <inheritdoc />
        public IRestClient CreateClient<TRestClient>(RestClientOptions options)
        {
            return new RestClient(
                nameof(TRestClient),
                _internalClientFactory,
                options,
                _mediaTypeFormatters,
                _telemetryFactory);
        }
    }
}