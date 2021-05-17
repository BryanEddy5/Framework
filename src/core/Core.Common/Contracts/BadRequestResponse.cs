using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Common.Contracts
{
    /// <summary>
    /// A class that it used to deserialize an http status code 400 to.
    /// This mirrors the shape of the the standard BadRequest produced by ASP .Net Core.
    /// </summary>
    public class BadRequestResponse
    {
        /// <summary>
        /// A message indicating the incoming request was bad.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// A list of model validation errors.
        /// </summary>
        public IDictionary<string, IList<string>>? ModelState { get; set; }
    }
}