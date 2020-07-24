using System;
using HumanaEdge.Webcore.Framework.Swagger.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HumanaEdge.Webcore.Framework.Swagger.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> for registering services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>s
        /// Registers Swagger in the <see cref="IServiceCollection"/> for creating API documentation.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>
        /// <typeparam name="TEntry">The <see cref="Type"/> of the Startup class.</typeparam>
        public static void AddSwaggerServices<TEntry>(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure services to grab OpenAPI settings from appropriate appsettings.json section.
            services.Configure<OpenApiConfigSettings>(configuration.GetSection(OpenApiConfigSettings.ConfigSettingsKey));

            // Configuration of SwaggerGen happens when ConfigureSwaggerOptions is injected.
            services.AddSwaggerGen();

            // Add Mvc convention to ensure ApiExplorer is enabled for all actions.
            services.Configure<MvcOptions>(c => c.Conventions.Add(new SwaggerApplicationConvention()));

            // https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters/issues/114
            // services.AddSwaggerExamplesFromAssemblyOf<ExampleViewModel>();

            // Register generator and it's dependencies
            services.AddTransient<ISwaggerProvider, SwaggerGenerator>();
            services.AddTransient<ISchemaGenerator, SchemaGenerator>();

            services.AddSwaggerGenNewtonsoftSupport();

            // Add and configure swagger via documented DI technique for complex options configuration
            // https://andrewlock.net/access-services-inside-options-and-startup-using-configureoptions/
            // https://benjamincollins.com/blog/using-dependency-injection-while-configuring-services/
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions<TEntry>>();
        }
    }
}