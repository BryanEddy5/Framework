using HumanaEdge.Webcore.ExampleWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Example.WebApi.Controllers
{
    /// <summary>
    /// Sample ApiController.
    /// </summary>
    [ApiController]
    [Route("weather")]
    public class WeatherForecastController
    {
        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// Ctor of controller that demonstrates dependency injection of IWeatherForecast.
        /// </summary>
        /// <param name="logger">An injected instance of <see cref="ILogger" /> from built in DI framework.</param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// The controller's Get (and default) method.
        /// </summary>
        /// <returns>An injected instance of <see cref="WeatherForecast" />.</returns>
        [HttpGet]
        public ActionResult<WeatherForecast> Get()
        {
            return new WeatherForecast();
        }
    }
}