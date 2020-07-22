using HumanaEdge.Webcore.Framework.DependencyInjection.Extensions;
using HumanaEdge.Webcore.Framework.Logging.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ExampleWebApi
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
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                .UseAppLogging<Startup>()
                .UseDependencyInjection<Startup>();
    }
}