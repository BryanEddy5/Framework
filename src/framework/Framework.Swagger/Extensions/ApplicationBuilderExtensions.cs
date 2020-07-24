using HumanaEdge.Webcore.Framework.Swagger.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace HumanaEdge.Webcore.Framework.Swagger.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder" /> for logging.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the application to use Swagger.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="config">
        /// The <see cref="IConfiguration" /> instance passed into the initial Startup.ConfigureService()
        /// call.
        /// </param>
        public static void UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // retrieve DocumentTitle from the original config settings
                    options.DocumentTitle = config.GetSection(OpenApiConfigSettings.ConfigSettingsKey)
                        .Get<OpenApiConfigSettings>()
                        .DocumentTitle;

                    // endpoint for swagger.json
                    options.SwaggerEndpoint("swagger/v1/swagger.json", $"{options.DocumentTitle} V1 (hardcoded)");

                    // default is all sections collapsed
                    options.DocExpansion(DocExpansion.None);

                    // configure UI to be at root
                    options.RoutePrefix = string.Empty;
                });
        }
    }
}