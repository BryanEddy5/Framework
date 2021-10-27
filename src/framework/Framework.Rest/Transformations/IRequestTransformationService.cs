using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Rest;

namespace HumanaEdge.Webcore.Framework.Rest.Transformations
{
    /// <summary>
    /// The service responsible for retrieving a token from either local memory cache or from a given endpoint.
    /// </summary>
    public interface IRequestTransformationService
    {
        /// <summary>
        /// Gets the access_token from either cache or the oauth endpoint.
        /// </summary>
        /// <typeparam name="TRestRequest">Type of the value to be returned.</typeparam>
        /// <param name="restRequest">The unique identifier for the token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>TRestRequest The access_token, either from cache or the oauth endpoint.</returns>
        public Task<TRestRequest> TransformRequest<TRestRequest>(
            TRestRequest restRequest,
            CancellationToken cancellationToken)
            where TRestRequest : RestRequest;

        /// <summary>
        /// Converts a <see cref="RestRequest{TRequest}" /> to <see cref="HttpRequestMessage" />.
        /// </summary>
        /// <param name="request">The rest request to be converted.</param>
        /// <typeparam name="TRequest">The <see cref="Type" /> of the request body.</typeparam>
        /// <returns>A http request to be sent.</returns>
        /// <exception cref="FormatFailedRestException">An exception thrown if the request body could not be serialized.</exception>
        public HttpRequestMessage ConvertToHttpRequestMessage<TRequest>(RestRequest<TRequest> request);

        /// <summary>
        /// Converts a <see cref="RestRequest" /> to a <see cref="HttpRequestMessage" />.
        /// </summary>
        /// <param name="request">The request to convert.</param>
        /// <returns>An <see cref="HttpRequestMessage" />.</returns>
        public HttpRequestMessage ConvertToHttpRequestMessage(RestRequest request);

        /// <summary>
        /// Test.
        /// </summary>
        /// <param name="httpResponseMessage">HTTP response message to be converted.</param>
        /// <returns>An <see cref="RestResponse" />.</returns>
        public Task<RestResponse> ConvertToRestResponse(HttpResponseMessage httpResponseMessage);

        /// <summary>
        /// Test.
        /// </summary>
        /// <param name="httpResponseMessage">HTTP response message to be converted.</param>
        /// <returns>An <see cref="FileResponse" />.</returns>
        public Task<FileResponse> ConvertToStreamResponse(HttpResponseMessage httpResponseMessage);
    }
}