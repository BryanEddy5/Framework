using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// The rest client sends RESTful http requests.
    /// </summary>
    /// <typeparam name="TClient">The type inheriting from this base class.</typeparam>
    public abstract class BaseRestClient<TClient> : IRestClient
        where TClient : BaseRestClient<TClient>
    {
        private readonly Lazy<IRestClient> _restClient;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="restClientFactory">A factory pattern for generating a <see cref="IRestClient" />.</param>
        /// <param name="options">Optional settings for configuring the rest client for each implementation.</param>
        public BaseRestClient(IRestClientFactory restClientFactory, RestClientOptions options)
        {
            _restClient = new Lazy<IRestClient>(() => restClientFactory.CreateClient<TClient>(options));
        }

        /// <inheritdoc />
        public async Task<RestResponse> SendAsync(RestRequest restRequest, CancellationToken cancellationToken)
        {
            return await _restClient.Value.SendAsync(restRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<RestResponse> SendAsync<TRequest>(
            RestRequest<TRequest> restRequest,
            CancellationToken cancellationToken)
        {
            return await _restClient.Value.SendAsync(restRequest, cancellationToken);
        }
    }
}