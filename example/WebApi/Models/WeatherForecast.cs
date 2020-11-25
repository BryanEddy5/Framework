using System;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Example.WebApi.DependencyInjection;

namespace HumanaEdge.Webcore.ExampleWebApi.Models
{
    /// <summary>
    /// A simple View Model that the WebApi uses. Demonstrates Swagger feature of Webcore.
    /// </summary>
    [DependencyInjectedComponent]
    public class WeatherForecast : IWeatherForecast
    {
        private static readonly Random Rng;

        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        static WeatherForecast()
        {
            Rng = new Random();
        }

        /// <summary>
        /// Designated ctor.
        /// </summary>
        public WeatherForecast()
        {
            Date = DateTime.UtcNow.AddDays(1);
            TemperatureC = Rng.Next(-20, 55);
            Summary = Summaries[Rng.Next(Summaries.Length)];
        }

        /// <summary>
        /// Property that demonstrates DateTime in the view model.
        /// </summary>
        public TestEnum SkyColor { get; set; }

        /// <summary>
        /// Property that demonstrates DateTime in the view model.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Property that demonstrates a string in the view model.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Property that demonstrates int in the view model.
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Property that demonstrates a getter in the view model that is calculated.
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}