using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Common.Extensions;
using HumanaEdge.Webcore.Core.Testing.Client;
using HumanaEdge.Webcore.Core.Testing.Transformations;
using RestSharp;

namespace HumanaEdge.Webcore.Framework.Testing.Integration.Client
{
    /// <inheritdoc />
    internal sealed class TestClient : ITestClient
    {
        private readonly IAsyncRequestTransformation[] _asyncRequestTransformations;

        private readonly IRequestTransformation[] _requestTransformations;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="restClient"> The client for sending restful requests. </param>
        /// <param name="asyncRequestTransformations"> Asynchronous request transformation services. </param>
        /// <param name="requestTransformations"> Request transformation services. </param>
        public TestClient(
            IRestClient restClient,
            IAsyncRequestTransformation[] asyncRequestTransformations,
            IRequestTransformation[] requestTransformations)
        {
            _asyncRequestTransformations = asyncRequestTransformations;
            _requestTransformations = requestTransformations;
            RestClient = restClient;
        }

        /// <inheritdoc />
        public IRestClient RestClient { get; }

        /// <inheritdoc />
        public async Task<IRestResponse> ExecuteAsync(IRestRequest restRequest)
        {
            await TransformRequest(restRequest);
            return await RestClient.ExecuteAsync(restRequest);
        }

        /// <inheritdoc />
        public async Task<IRestResponse<TResponse>> ExecuteAsync<TResponse>(IRestRequest restRequest)
        {
            await TransformRequest(restRequest);
            return await RestClient.ExecuteAsync<TResponse>(restRequest);
        }

        private async Task TransformRequest(IRestRequest restRequest)
        {
            foreach (var transformation in _asyncRequestTransformations)
            {
                await transformation.ExecuteAsync(restRequest);
            }

            _requestTransformations.ForEach(x => x.Execute(restRequest));
        }
    }
}