using System.Net;
using System.Net.Http;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// The base rest response that contains base properties common to all responses.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public abstract class BaseRestResponse
    {
        /// <summary>
        /// The base ctor.
        /// </summary>
        /// <param name="isSuccessful">A flag indicating whether or not the request successfully executed.</param>
        /// <param name="statusCode">The status code of the response.</param>
        public BaseRestResponse(bool isSuccessful, HttpStatusCode statusCode)
        {
            IsSuccessful = isSuccessful;
            StatusCode = statusCode;
        }

        /// <summary>
        /// And indicator if the request was successful.
        /// </summary>
        public bool IsSuccessful { get; }

        /// <summary>
        /// The status code returned from the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }
    }
}