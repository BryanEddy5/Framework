using HumanaEdge.Webcore.Example.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HumanaEdge.Webcore.Example.WebApi.Controllers
{
    /// <summary>
    /// A test controller for deserializing bad requests.
    /// </summary>
    [ApiController]
    [Route(Route)]
    public class BadRequestController : ControllerBase
    {
        /// <summary>
        /// The route template.
        /// </summary>
        public const string Route = "badRequest";

        /// <summary>
        /// Returns a bad request.
        /// </summary>
        /// <param name="foo">A complex object to validate.</param>
        /// <returns>Bad Request.</returns>
        [HttpPost]
        public ActionResult<Foo> ReturnBadRequest(Foo foo)
        {
            return BadRequest(ModelState);
        }
    }
}