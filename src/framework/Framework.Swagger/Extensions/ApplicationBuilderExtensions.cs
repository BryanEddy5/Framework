using System;
using HumanaEdge.Webcore.Framework.Swagger.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
        /// <param name="config">The application configuration.</param>
        /// <param name="provider">The api version detail provider.</param>
        public static void UseSwaggerDocumentation(
            this IApplicationBuilder app,
            IConfiguration config,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.DocumentTitle = config.GetSection(OpenApiConfigSettings.ConfigSettingsKey)
                        .Get<OpenApiConfigSettings>()
                        .DocumentTitle;

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        var endpoint = $"/swagger/{description.ApiVersion}/swagger.json";
                        var groupName = $"{options.DocumentTitle} {description.GroupName.ToUpperInvariant()}";
                        options.SwaggerEndpoint(endpoint, groupName);
                    }

                    options.DocExpansion(DocExpansion.None);
                    options.RoutePrefix = string.Empty;
                });
        }
    }
}