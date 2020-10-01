using System;
using System.Net;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// The the response from the rest request that contains.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class RestResponse : BaseRestResponse
    {
        /// <summary>
        /// A service for deserializing the response from a http request.
        /// </summary>
        private readonly IRestResponseDeserializer _restResponseDeserializer;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="isSuccessful">An indicator if the request was successful.</param>
        /// <param name="restResponseDeserializer">A service to deserializing the response.</param>
        /// <param name="statusCode">The status code associated with the rest response.</param>
        /// <param name="location">The location header from the http response.</param>
        public RestResponse(
            bool isSuccessful,
            IRestResponseDeserializer restResponseDeserializer,
            HttpStatusCode statusCode,
            Uri? location = null)
            : base(isSuccessful, statusCode)
        {
            _restResponseDeserializer = restResponseDeserializer;
            Location = location;
        }

        /// <summary>
        /// The location header from the http response.
        /// </summary>
        public Uri? Location { get; }

        /// <summary>
        /// The http response as a byte array allowing for inspection of the response before deserializing.
        /// </summary>
        public byte[] ResponseBytes => _restResponseDeserializer.ResponseBytes;

        /// <summary>
        /// Converts the RESTful response to a designated <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type" /> to be deserialized to.</typeparam>
        /// <returns>The designated <see cref="Type" /> to be deserialized.</returns>
        public TResponse ConvertTo<TResponse>()
        {
            return _restResponseDeserializer.ConvertTo<TResponse>();
        }
    }
}