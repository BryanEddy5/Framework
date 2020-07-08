using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Web;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    internal sealed class RestClientFactory : IRestClientFactory
    {
        /// <summary>
        ///     A factory for generating a rest client.
        /// </summary>
        private readonly IInternalClientFactory _internalClientFactory;

        /// <summary>
        ///     A collection of media types for formatting a request.
        /// </summary>
        private readonly IMediaTypeFormatter[] _mediaTypeFormatters;

        /// <summary>
        ///     Accessing the request id of the incoming request.
        /// </summary>
        private readonly IRequestIdAccessor? _requestIdAccessor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RestClientFactory" /> class.
        /// </summary>
        /// <param name="internalClientFactory">A factory for generating a rest client.</param>
        /// <param name="mediaTypeFormatters">A collection of media types for formatting a request.</param>
        /// <param name="requestIdAccessor">Accessing the request id of the incoming request.</param>
        public RestClientFactory(
            IInternalClientFactory internalClientFactory,
            IEnumerable<IMediaTypeFormatter> mediaTypeFormatters,
            IRequestIdAccessor? requestIdAccessor = null)
        {
            _internalClientFactory = internalClientFactory;
            _requestIdAccessor = requestIdAccessor;
            _mediaTypeFormatters = mediaTypeFormatters.ToArray();
        }

        /// <inheritdoc />
        public IRestClient CreateClient<TRestClient>(RestClientOptions options)
        {
            return new RestClient(
                nameof(TRestClient),
                _internalClientFactory,
                _requestIdAccessor,
                options,
                _mediaTypeFormatters);
        }
    }
}