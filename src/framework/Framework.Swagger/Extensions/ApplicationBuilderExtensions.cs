using HumanaEdge.Webcore.Framework.Swagger.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        /// <param name="provider">The api version provider, for api versioning support.</param>
        /// <param name="logger">The application logger.</param>
        public static void UseSwaggerDocumentation(
            this IApplicationBuilder app,
            IConfiguration config,
            IApiVersionDescriptionProvider provider,
            ILogger logger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // retrieve DocumentTitle from the original config settings
                    options.DocumentTitle = config.GetSection(OpenApiConfigSettings.ConfigSettingsKey)
                        .Get<OpenApiConfigSettings>()
                        .DocumentTitle;

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        var groupName = $"{options.DocumentTitle} {description.GroupName.ToUpperInvariant()}";
                        var endpoint = $"/swagger/{description.GroupName}/swagger.json";
                        options.SwaggerEndpoint(endpoint, groupName);
                    }

                    // default is all sections collapsed
                    options.DocExpansion(DocExpansion.None);

                    // configure UI to be at root
                    options.RoutePrefix = string.Empty;
                });
        }
    }
}