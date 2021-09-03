using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Domain;
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
        private readonly ICatFactsService _catFactsService;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="catFactsService">A service for getting a random cat fact.</param>
        public CatFactsController(ICatFactsService catFactsService)
        {
            _catFactsService = catFactsService;
        }

        /// <summary>
        /// Retrieves a random cat fact.
        /// </summary>
        /// <param name="cancellationToken">The request cancellation token.</param>
        /// <param name="forceRefresh">Force refresh the cache. </param>
        /// <returns>A random, fun cat fact.</returns>
        [HttpGet]
        public async Task<ActionResult<RandomCatFactResponse?>> Get(CancellationToken cancellationToken, bool forceRefresh = false)
        {
            var catFact = await _catFactsService.GetAsync(cancellationToken, forceRefresh);
            return catFact?.ToRandomCatFactResponse();
        }
    }
}