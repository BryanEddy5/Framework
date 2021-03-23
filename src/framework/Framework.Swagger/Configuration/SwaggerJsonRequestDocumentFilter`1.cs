using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HumanaEdge.Webcore.Framework.Swagger.Configuration
{
    /// <summary>
    /// An implementation of <see cref="IDocumentFilter" /> for configuring Swagger document filters.
    /// </summary>
    /// <typeparam name="TEntry">generic type of ConfigureSwaggerOptions that configured this filter.</typeparam>
    internal sealed class SwaggerJsonRequestDocumentFilter<TEntry> : IDocumentFilter
    {
        private static bool _runOnce;

        private readonly OpenApiConfigSettings _configSettings;

        private readonly string _currentServiceHostUri;

        /// <summary>
        /// designated ctor.
        /// </summary>
        /// <param name="contextAccessor"><see cref="IHttpContextAccessor" />.</param>
        /// <param name="configSettings">A typed IOptions instance.</param>
        public SwaggerJsonRequestDocumentFilter(
            IHttpContextAccessor contextAccessor,
            IOptions<OpenApiConfigSettings> configSettings)
        {
            var request = contextAccessor.HttpContext!.Request;
            _currentServiceHostUri = $"{request.Scheme}://{request.Host}{request.PathBase}";
            _configSettings = configSettings.Value;
        }

        /// <summary>
        /// Applies filters to the logical Swagger document. This is called every time the Swagger UI page is refreshed.
        /// We have to be careful that swaggerDoc.Info.Description is only modified once. There seems to be a bug in Swagger.
        /// It seems to be a singleton that we continuously add to on every refresh. That's why we get repeating elements.
        /// </summary>
        /// <param name="swaggerDoc">The logical Swagger document.</param>
        /// <param name="context"><see cref="DocumentFilterContext" />.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = _configSettings.NonProdServerBaseAndSuffix },
                new OpenApiServer { Url = $"{_configSettings.NonProdServerBaseAndSuffix}-sit" },
                new OpenApiServer { Url = $"{_configSettings.NonProdServerBaseAndSuffix}-uat" },
                new OpenApiServer { Url = _configSettings.ProductionServerBaseAndSuffix }
            };
            if (_currentServiceHostUri.Contains("localhost"))
            {
                // or remind if on localhost
                swaggerDoc.Servers.Insert(0, new OpenApiServer { Url = _currentServiceHostUri });
            }

            // swaggerDoc.Info.Description does the wrong thing upon refreshes. Can only run once.
            // Using double test for efficiency. Similar to how a singleton is safely initialized in a multi-threaded environment.
            if (!_runOnce)
            {
                lock (this)
                {
                    if (!_runOnce)
                    {
                        _runOnce = true;

                        // only add to description "git: a298n3, pipeline: 39872" if the URL has "np" in it.
                        if (_currentServiceHostUri.Contains("np"))
                        {
                            swaggerDoc.Info.Description += ConfigureSwaggerOptions<TEntry>.BuildSourceInfo;
                        }
                        else if (_currentServiceHostUri.Contains("localhost"))
                        {
                            swaggerDoc.Info.Description += " @<b>localhost</b>";
                        }

                        // show link to API doc
                        var docUrl = _configSettings.ServiceDocumentationUrl;
                        swaggerDoc.Info.Description += $"<p><a href={docUrl}>{docUrl}</a><p>";

                        // add any extra description
                        if (!string.IsNullOrEmpty(_configSettings.AdditionalDescription))
                        {
                            swaggerDoc.Info.Description += "<p>" + _configSettings.AdditionalDescription;
                        }
                    }
                }
            }
        }
    }
}