using System;
using FluentValidation.AspNetCore;
using HumanaEdge.Webcore.Framework.Logging.Extensions;
using HumanaEdge.Webcore.Framework.Rest.Extensions;
using HumanaEdge.Webcore.Framework.Web.Exceptions;
using HumanaEdge.Webcore.Framework.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;

namespace HumanaEdge.Webcore.Framework.Web
{
    /// <summary>
    ///     A common base startup class for bootstrapping the application.
    /// </summary>
    /// <typeparam name="TStartup">The <see cref="Type" /> of the Startup class.</typeparam>
    public abstract class BaseStartup<TStartup>
        where TStartup : BaseStartup<TStartup>
    {
        /// <summary>
        ///     Designated ctor.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        protected BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///     The application configuration settings.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     Configures the web application by building out the pipeline and configuring various settings.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="logger">The application logger.</param>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<TStartup> logger)
        {
            _ = env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseHsts();

            app.UseRequestMiddleware()
                .UseMiddleware<ExceptionHandlingMiddleware>()
                .UseLoggingContextMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseReadyHealthChecks();
            app.UseTracing(Configuration);

            // app specific middleware
            ConfigureApp(app, env, logger);

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/health");
                });

            logger.LogInformation("application started");
        }

        /// <summary>
        ///     Configures a DI container and a <see cref="IServiceProvider" /> from a given <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of service registrations that should be included in the container.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddRequestIdAccessor();
            services.AddOptionsPattern(Configuration);

            services.AddControllers(options => options.AddFilters(ConfigureFilters()))
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    })
                .AddFluentValidation()
                .AddControllersAsServices();

            var httpClientBuilder = ConfigureAppServices(services);
            services.AddTracing(Configuration, httpClientBuilder);

            services.AddRestClient();
        }

        /// <summary>
        ///     Allow for the application to register app specific middleware.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="logger">The application logger.</param>
        protected virtual void ConfigureApp(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<TStartup> logger)
        {
            // nop
        }

        /// <summary>
        ///     Hook for applications to add app-specific services to the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The running service collection.</param>
        /// <returns>returns an <see cref="IHttpClientBuilder" /> for tracing to be attached to.</returns>
        protected virtual IHttpClientBuilder ConfigureAppServices(IServiceCollection services)
        {
            return services.AddHttpClient("backend");
        }

        /// <summary>
        ///     Hook for applications to add app-specific filters to the MVC pipeline.
        /// </summary>
        /// <returns>
        ///     The collection of filters, aliased by type, that should be added to the pipeline (in the order they should be
        ///     added.
        /// </returns>
        protected virtual Type[] ConfigureFilters()
        {
            return Array.Empty<Type>();
        }
    }
}