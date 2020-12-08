using System.Diagnostics.CodeAnalysis;
using HumanaEdge.Webcore.Example.WebApi;
using HumanaEdge.Webcore.Framework.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

[assembly: ExcludeFromCodeCoverage]

namespace HumanaEdge.Webcore.ExampleWebApi
{
    /// <summary>
    /// Standard Program class containing Main entry point.
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config.AddConfigOptions(args);
                    })
                .UseCustomHostBuilder<Startup>()
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
    }
}