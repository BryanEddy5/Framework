using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Services;
using HumanaEdge.Webcore.Example.WebApi.Contracts;
using HumanaEdge.Webcore.Example.WebApi.Converters;
using Microsoft.AspNetCore.Mvc;

namespace HumanaEdge.Webcore.Example.WebApi.Controllers
{
    /// <summary>
    /// A controller with cat fact resources.
    /// </summary>
    [ApiController]
    [Route("api/catfacts")]
    public class CatFactsController : ControllerBase
    {
        /// <summary>
        /// A service for getting a random cat fact.
        /// </summary>
        private readonly IRandomCatFactService _randomCatFactService;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="randomCatFactService">A service for getting a random cat fact.</param>
        public CatFactsController(IRandomCatFactService randomCatFactService)
        {
            _randomCatFactService = randomCatFactService;
        }

        /// <summary>
        /// Retrieves a random cat fact.
        /// </summary>
        /// <param name="cancellationToken">The request cancellation token.</param>
        /// <returns>A random, fun cat fact.</returns>
        [HttpGet]
        public async Task<ActionResult<RandomCatFactResponse>> Get(CancellationToken cancellationToken)
        {
            var catFact = await _randomCatFactService.GetAsync(cancellationToken);
            return catFact.ToRandomCatFactResponse();
        }
    }
}