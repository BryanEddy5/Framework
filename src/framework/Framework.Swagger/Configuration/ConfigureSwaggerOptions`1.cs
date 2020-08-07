using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HumanaEdge.Webcore.Framework.Swagger.Configuration
{
    /// <summary>
    /// configuration settings for API documentation middleware. Uses this technique to DI the
    /// options configuration correctly in the bootstrap process.
    /// https://andrewlock.net/access-services-inside-options-and-startup-using-configureoptions/
    /// https://benjamincollins.com/blog/using-dependency-injection-while-configuring-services/ .
    /// </summary>
    /// <typeparam name="TEntry">The <see cref="Type" /> of the Startup class.</typeparam>
    internal sealed class ConfigureSwaggerOptions<TEntry> : IConfigureOptions<SwaggerGenOptions>
    {
        private static readonly OpenApiInfo ApiInfo;

        private static readonly Assembly EntryAssembly;

        private static readonly string XmlPath;

        static ConfigureSwaggerOptions()
        {
            EntryAssembly = typeof(TEntry).Assembly;
            var xmlFile = $"{EntryAssembly.GetName().Name}.xml";
            XmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            BuildSourceInfo = GetBuildSourceInfo();
            ApiInfo = CreateInfo();
        }

        /// <summary>
        /// The build and pipeline info to display for non-prod.  Used in SwaggerJsonRequestDocumentFilter&lt;T&gt;.Apply().
        /// </summary>
        public static string BuildSourceInfo { get; }

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // This configures a filter that is executed every time the Swagger UI page is refreshed.
            // This is useful because the filter gets access to the HttpContext and allows us to detect
            // the environment that we are currently running in (e.g., prod vs non-prod).
            // But it's easy to create bugs since the filter can modify existing state.
            options.DocumentFilter<SwaggerJsonRequestDocumentFilter<TEntry>>();

            // Enable the request/response examples
            // options.ExampleFilters();

            // CustomSchemaIds->FullName enables UseFullTypeNameInSchemaIds to prevent this:
            // https://stackoverflow.com/questions/46071513/swagger-error-conflicting-schemaids-duplicate-schemaids-detected-for-types-a-a
            // options.CustomSchemaIds(x => x.FullName);

            // Set the comments path for the Swagger JSON and UI.
            options.IncludeXmlComments(XmlPath);

            options.AddSecurityDefinition(
                "apikey",
                new OpenApiSecurityScheme
                {
                    Description = "Apigee API key",
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "apikey"
                            }
                        },
                        new List<string>()
                    }
                });

            // Configures the initial hero with OpenApiInfo.
            // For some reason (and it's not obvious), this first parameter needs to match OpenApiInfo.Version below.
            options.SwaggerDoc("v1", ApiInfo);

            // add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();
        }

        /// <summary>
        /// Configures initial hero information.
        /// </summary>
        /// <returns><see cref="OpenApiInfo" />.</returns>
        internal static OpenApiInfo CreateInfo()
        {
            var info = new OpenApiInfo
            {
                Version = "v1",
                Title = EntryAssembly?.GetCustomAttribute<AssemblyProductAttribute>()?.Product,
                Description = EntryAssembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description

                // Contact = new OpenApiContact() { Name = "George Chung", Email = "george.chung@nortal.com" },
                // TermsOfService = new Uri("https://opensource.org/licenses/MIT"),
                // License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            };
            return info;
        }

        /// <summary>
        /// Extracts build info from the assembly for non-prod environments.
        /// </summary>
        /// <returns>string representing formatted build info.</returns>
        private static string GetBuildSourceInfo()
        {
            // eg "c81631b9_1193"
            var informationalVersion = EntryAssembly
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion
                .Split("_")
                .ToList();
            var gitHash = informationalVersion?.First();
            var pipelineId = informationalVersion?.Last();

            return $"<p><b>git:</b> {gitHash}, <b>pipeline id:</b> {pipelineId}";
        }
    }
}