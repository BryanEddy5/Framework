using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Web.Contracts
{
    /// <summary>
    /// A class that it used to deserialize an http status code 400 to.
    /// This mirrors the shape of the the standard BadRequest produced by ASP .Net Core.
    /// </summary>
    public class BadRequestResponse : BaseErrorResponse
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="title">A message indicating the incoming request was bad.</param>
        /// <param name="traceId">The requests unique identifier.</param>
        /// <param name="status">The returned result status code associated with the error.</param>
        /// <param name="errors">A list of model validation errors.</param>
        /// <param name="type">The type of response.</param>
        public BadRequestResponse(
            string title,
            string traceId,
            int status,
            IReadOnlyDictionary<string, IReadOnlyList<string>>? errors,
            string? type)
            : base(title, traceId, status)
        {
            Errors = errors;
            Type = type;
        }

        /// <summary>
        /// A list of model validation errors.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>>? Errors { get; }

        /// <summary>
        /// The type of response.
        /// </summary>
        public string? Type { get; }
    }
}