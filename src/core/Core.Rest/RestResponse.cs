using System;
using System.Net;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    ///     The the response from the rest request that contains.
    /// </summary>
    public sealed class RestResponse
    {
        /// <summary>
        ///     A service for deserializing the response from a http request.
        /// </summary>
        private readonly IRestResponseDeserializer _restResponseDeserializer;

        /// <summary>
        ///     Designated ctor.
        /// </summary>
        /// <param name="isSuccessful">An indicator if the request was successful.</param>
        /// <param name="restResponseDeserializer">A service to deserializing the response.</param>
        /// <param name="statusCode">The status code associated with the rest response.</param>
        public RestResponse(
            bool isSuccessful,
            IRestResponseDeserializer restResponseDeserializer,
            HttpStatusCode statusCode)
        {
            IsSuccessful = isSuccessful;
            _restResponseDeserializer = restResponseDeserializer;
            StatusCode = statusCode;
        }

        /// <summary>
        ///     And indicator if the request was successful.
        /// </summary>
        public bool IsSuccessful { get; }

        /// <summary>
        ///     The http response as a byte array allowing for inspection of the response before deserializing.
        /// </summary>
        public byte[] ResponseBytes => _restResponseDeserializer.ResponseBytes;

        /// <summary>
        ///     The status code returned from the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        ///     Converts the RESTful response to a designated <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type" /> to be deserialized to.</typeparam>
        /// <returns>The designated <see cref="Type" /> to be deserialized.</returns>
        public TResponse ConvertTo<TResponse>()
        {
            return _restResponseDeserializer.ConvertTo<TResponse>();
        }
    }
}