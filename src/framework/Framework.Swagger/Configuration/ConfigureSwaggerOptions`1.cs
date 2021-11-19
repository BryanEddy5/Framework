using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HumanaEdge.Webcore.Framework.Swagger.Configuration
{
    /// <summary>
    /// configuration settings for API documentation middleware. Uses this technique to DI the
    /// options configuration correctly in the bootstrap process.
    /// </summary>
    /// <typeparam name="TEntry">The <see cref="Type" /> of the Startup class.</typeparam>
    internal sealed class ConfigureSwaggerOptions<TEntry> : IConfigureOptions<SwaggerGenOptions>
    {
        /// <summary>
        /// A reference to the entry point this API is using swagger within.
        /// </summary>
        private static readonly Assembly EntryAssembly;

        /// <summary>
        /// The path to the xml-docs for swagger's output.
        /// </summary>
        private static readonly string XmlPath;

        /// <inheritdoc cref="IApiVersionDescriptionProvider"/>
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ConfigureSwaggerOptions()
        {
            EntryAssembly = typeof(TEntry).Assembly;
            var xmlFile = $"{EntryAssembly.GetName().Name}.xml";
            XmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            BuildSourceInfo = GetBuildSourceInfo();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The api version provider.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// The build and pipeline info to display for non-prod.
        /// </summary>
        public static string BuildSourceInfo { get; }

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            options.OperationFilter<SwaggerDefaultValues>();
            options.DocumentFilter<SwaggerJsonRequestDocumentFilter<TEntry>>();
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
            options.AddSecurityRequirement(CreateSecurityRequirement());
            AddVersionedDocs(options);
        }

        /// <summary>
        /// Generates the <see cref="OpenApiInfo"/> version information for this description.
        /// </summary>
        /// <param name="description">The <see cref="ApiVersionDescription"/> data.</param>
        /// <returns>The generated version information.</returns>
        private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Version = $"v{description.ApiVersion.MajorVersion}",
                Title = EntryAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product,
                Description = EntryAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        /// <summary>
        /// Generates the <see cref="OpenApiSecurityRequirement"/> for this swagger documentation.
        /// </summary>
        /// <returns>The security requirement.</returns>
        private static OpenApiSecurityRequirement CreateSecurityRequirement()
        {
            return new OpenApiSecurityRequirement
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
            };
        }

        /// <summary>
        /// Extracts build info from the assembly for non-prod environments.
        /// </summary>
        /// <returns>string representing formatted build info.</returns>
        private static string GetBuildSourceInfo()
        {
            var informationalVersion = EntryAssembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion
                .Split("_")
                .ToList();
            var gitHash = informationalVersion?.First();
            var pipelineId = informationalVersion?.Last();

            return $"<p><b>git:</b> {gitHash}, <b>pipeline id:</b> {pipelineId}";
        }

        /// <summary>
        /// Adds the actual swagger documents to the swagger UI.
        /// </summary>
        /// <param name="options">The swagger generation options.</param>
        private void AddVersionedDocs(SwaggerGenOptions options)
        {
            // if we are using API Versioning and have multiple versions, then add them all.
            if (_provider.ApiVersionDescriptions.Count > 1)
            {
                foreach (var description in _provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        description.GroupName,
                        CreateVersionInfo(description));
                }
            }

            // if we do not have multiple versions, then add just the one.
            else
            {
                var description = _provider.ApiVersionDescriptions[0];
                var versionInfo = CreateVersionInfo(description);
                options.SwaggerDoc(description.GroupName, versionInfo);
            }
        }
    }
}