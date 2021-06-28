using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <inheritdoc />
    /// <typeparam name="TClient">The type inheriting from this base class.</typeparam>
    public abstract class BaseRestClient<TClient> : IRestClient
        where TClient : BaseRestClient<TClient>
    {
        /// <inheritdoc cref="IRestClient"/>
        private readonly Lazy<IRestClient> _restClient;

        /// <summary>
        /// Constructor for when the <see cref="RestClientOptions"/> is a static object.
        /// </summary>
        /// <param name="restClientFactory">A factory pattern for generating a <see cref="IRestClient" />.</param>
        /// <param name="options">Optional settings for configuring the rest client for each implementation.</param>
        protected BaseRestClient(IRestClientFactory restClientFactory, RestClientOptions options)
        {
            _restClient = new Lazy<IRestClient>(() => restClientFactory.CreateClient<TClient>(options));
        }

        /// <summary>
        /// Constructor for when the <see cref="RestClientOptions"/> is a dynamic object.
        /// </summary>
        /// <param name="restClientFactory">A factory pattern for generating a <see cref="IRestClient" />.</param>
        /// <param name="optionsFactory">A factory for generating the <see cref="RestClientOptions"/>.</param>
        protected BaseRestClient(IRestClientFactory restClientFactory,  Func<TClient, RestClientOptions> optionsFactory)
        {
            _restClient = new Lazy<IRestClient>(
                () => restClientFactory.CreateClient<TClient>(optionsFactory((TClient)this)));
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

        /// <inheritdoc />
        public async Task<FileResponse> GetFileAsync(RestRequest restRequest, CancellationToken cancellationToken)
        {
            return await _restClient.Value.GetFileAsync(restRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<FileResponse> GetFileAsync<TRequest>(
            RestRequest<TRequest> restRequest,
            CancellationToken cancellationToken)
        {
            return await _restClient.Value.GetFileAsync(restRequest, cancellationToken);
        }
    }
}