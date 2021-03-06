using System;

namespace HumanaEdge.Webcore.Example.WebApi.DependencyInjection
{
    /// <summary>
    /// sample interface for testing webcore's dependency injection.
    /// </summary>
    public interface IWeatherForecast
    {
        /// <summary>
        /// Date.
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// Temperature in Celcius.
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Weather Forecast Summary.
        /// </summary>
        public string Summary { get; set; }
    }
}