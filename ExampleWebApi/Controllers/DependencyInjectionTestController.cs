using ExampleWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleWebApi.Controllers
{
    /// <summary>
    /// Sample ApiController.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DependencyInjectionTestController
    {
        private readonly ITransientService _transientService;
        private readonly IScopedService _scopedService;
        private readonly ISingletonService _singletonService;

        /// <summary>
        /// Ctor of controller that demonstrates dependency injection of IWeatherForecast.
        /// </summary>
        /// <param name="transientService">An injected instance of<see cref="ITransientService"/> from webcore's DI framework.</param>
        /// <param name="scopedService">An injected instance of<see cref="IScopedService"/> from webcore's DI framework.</param>
        /// <param name="singletonService">An injected instance of<see cref="ISingletonService"/> from webcore's DI framework.</param>
        public DependencyInjectionTestController(ITransientService transientService, IScopedService scopedService, ISingletonService singletonService)
        {
            _transientService = transientService;
            _scopedService = scopedService;
            _singletonService = singletonService;
        }

        /// <summary>
        /// The controller's Get (and default) method.
        /// </summary>
        /// <returns>boolean indicating success or failure.</returns>
        [HttpGet]
        public bool Get()
        {
            if (!(_transientService != null && _scopedService != null && _singletonService != null))
            {
                return false;
            }

            return true;
        }
    }
}