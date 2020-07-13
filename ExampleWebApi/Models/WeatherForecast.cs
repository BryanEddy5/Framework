using System;
using HumanaEdge.Webcore.Core.DependencyInjection;

namespace ExampleWebApi.Models
{
    /// <summary>
    /// A simple View Model that the ExampleWebApi uses.
    /// Demonstrates Dependency Injection feature of Webcore.
    /// Demonstrates Swagger feature of Webcore.
    /// </summary>
    [DependencyInjectedComponent]
    public class WeatherForecast : IWeatherForecast
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly Random Rng;

        static WeatherForecast()
        {
            Rng = new Random();
        }

        /// <summary>
        /// Designated ctor.
        /// </summary>
        public WeatherForecast()
        {
            Date = DateTime.Now.AddDays(1);
            TemperatureC = Rng.Next(-20, 55);
            Summary = Summaries[Rng.Next(Summaries.Length)];
        }

        /// <summary>
        /// Property that demonstrates DateTime in the view model.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Property that demonstrates int in the view model.
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Property that demonstrates a getter in the view model that is calculated.
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Property that demonstrates a string in the view model.
        /// </summary>
        public string Summary { get; set; }
    }
}