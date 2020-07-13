using ExampleWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExampleWebApi.Controllers
{
    /// <summary>
    /// Sample ApiController.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IWeatherForecast _weatherForecast;

        /// <summary>
        /// Ctor of controller that demonstrates dependency injection of IWeatherForecast.
        /// </summary>
        /// <param name="logger">An injected instance of <see cref="ILogger"/> from built in DI framework.</param>
        /// <param name="weatherForecast">An injected instance of<see cref="IWeatherForecast"/> from webcore's DI framework.</param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecast weatherForecast)
        {
            _logger = logger;
            _weatherForecast = weatherForecast;
        }

        /// <summary>
        /// The controller's Get (and default) method.
        /// </summary>
        /// <returns>An injected instance of <see cref="IWeatherForecast"/>.</returns>
        [HttpGet]
        public IWeatherForecast Get()
        {
            var a = _weatherForecast;
            return a;
        }
    }
}