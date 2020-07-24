using System.Threading.Tasks;
using ExampleWebApi;
using FluentAssertions.Json;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Logging.Extensions;
using HumanaEdge.Webcore.Framework.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Tests
{
    /// <summary>
    /// Unit tests for <see cref="DependencyInjection" />.
    /// </summary>
    public class SwaggerTests : BaseTests
    {
        private const string SwaggerPath = "swagger/v1/swagger.json";

        private readonly IHost _host;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public SwaggerTests()
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .UseAppLogging<Startup>()
                .ConfigureWebHostDefaults(
                    builder =>
                    {
                        // Will run the Kestrel server in-process
                        // and provide a factory for test clients
                        builder.UseTestServer();
                        builder.ConfigureAppConfiguration(
                            (hostingContext, config) =>
                            {
                                config.AddConfigOptions();
                            });

                        // This is ExampleWebApi's Startup...
                        // And whose build generated xml documentation...
                        // So this test is dependent on that server build
                        builder.UseStartup<Startup>();
                    });

            _host = hostBuilder.Start();
        }

        /// <summary>
        /// Validates the results of a request to the swagger.json endpoint.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateSchema()
        {
            await AssertSwaggerJsonMatchesExpected(
                    "$.components.schemas.WeatherForecast",
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"
            {
                'type': 'object',
                'properties': {
                    'skyColor': {
                        '$ref': '#/components/schemas/TestEnum'
                    },
                    'date': {
                        'type': 'string',
                        'description': 'Property that demonstrates DateTime in the view model.',
                        'format': 'date-time'
                    },
                    'temperatureC': {
                        'type': 'integer',
                        'description': 'Property that demonstrates int in the view model.',
                        'format': 'int32',
                    },
                    'temperatureF': {
                        'type': 'integer',
                        'description': 'Property that demonstrates a getter in the view model that is calculated.',
                        'format': 'int32',
                        'readOnly': true,
                    },
                    'summary': {
                        'type': 'string',
                        'description': 'Property that demonstrates a string in the view model.',
                        'nullable': true,
                    }
                },
                'additionalProperties': false,
                'description': 'A simple View Model that the ExampleWebApi uses. Demonstrates Swagger feature of Webcore.'
            }")
#pragma warning restore SA1118 // Parameter should not span multiple lines
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the results of a request to the swagger.json endpoint.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateSecuritySchemesSection()
        {
            await AssertSwaggerJsonMatchesExpected(
                    "$.components.securitySchemes",
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"
            {
                'apikey': {
                  'type': 'apiKey',
                  'description': 'Apigee API key',
                  'name': 'x-api-key',
                  'in': 'header'
                }
            }")
#pragma warning restore SA1118 // Parameter should not span multiple lines
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the results of a request to the swagger.json endpoint.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateSecuritySection()
        {
            await AssertSwaggerJsonMatchesExpected(
                    "$.security",
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"
            [
                {
                    'apikey': [ ]
                }
            ]")
#pragma warning restore SA1118 // Parameter should not span multiple lines
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the results of a request to the swagger.json endpoint.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateEndpointJson()
        {
            await AssertSwaggerJsonMatchesExpected(
                    "$.paths./api/v1/WeatherForecast",
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"
            {
                'get': {
                    'tags': [
                        'WeatherForecast'
                    ],
                    'summary': 'The controller\'s Get (and default) method.',
                    'responses': {
                        '200': {
                            'description': 'Success',
                            'content': {
                            'application/json': {
                                'schema': {
                                    '$ref': '#/components/schemas/WeatherForecast'
                                    }
                                },
                            }
                        }
                    }
                }
             }")
#pragma warning restore SA1118 // Parameter should not span multiple lines
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the results of a request to the swagger.json endpoint.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateEnumAsString()
        {
            await AssertSwaggerJsonMatchesExpected(
                    "$.components.schemas.TestEnum",
#pragma warning disable SA1118 // Parameter should not span multiple lines
                    @"
            {
                'enum': [
                    'Red',
                    'Yellow',
                    'Green',
                    'Blue'
                ],
                'type': 'string',
                'description': 'TestEnum is used to test ""treat enums as strings"" in our APIs, both controller methods and swagger doc.'
            }")
#pragma warning restore SA1118 // Parameter should not span multiple lines
                .ConfigureAwait(false);
        }

        private async Task AssertSwaggerJsonMatchesExpected(string actualSwaggerJpath, string expectedJson)
        {
            var actualValidateResult = await GetSwaggerJObject(actualSwaggerJpath);
            actualValidateResult.Should().BeEquivalentTo(expectedJson);
        }

        private async Task<JToken> GetSwaggerJObject(string jpathSelector)
        {
            var client = _host.GetTestClient();
            var swaggerResponseMessage = await client.GetAsync(SwaggerPath);
            var swaggerJObject = JObject.Parse(await swaggerResponseMessage.Content.ReadAsStringAsync());
            var actualValidateResult = swaggerJObject.SelectToken(jpathSelector);
            return actualValidateResult;
        }
    }
}