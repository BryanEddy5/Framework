using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Web.Contracts
{
    /// <summary>
    /// A class that it used to deserialize an http status code 400 to.
    /// This mirrors the shape of the the standard BadRequest produced by ASP .Net Core.
    /// </summary>
    public class BadRequestResponse
    {
        /// <summary>
        /// A list of model validation errors.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>>? Errors { get; set; }

        /// <summary>
        /// The returned status code.
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// A message indicating the incoming request was bad.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The requests unique identifier.
        /// </summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// The type of response.
        /// </summary>
        public string? Type { get; set; }
    }
}