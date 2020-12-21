using System.Threading.Tasks;
using FluentAssertions.Json;
using HumanaEdge.Webcore.Example.WebApi;
using HumanaEdge.Webcore.ExampleWebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Swagger.Tests
{
    /// <summary>
    /// Unit tests for <see cref="DependencyInjection" />.
    /// </summary>
    public class SwaggerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string SwaggerPath = "swagger/v1/swagger.json";

        private readonly WebApplicationFactory<Startup> _factory;

        /// <summary>
        /// Common test setup.
        /// </summary>
        /// <param name="factory">Creates a client for testing.</param>
        public SwaggerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
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
                'description': 'A simple View Model that the WebApi uses. Demonstrates Swagger feature of Webcore.'
            }");
#pragma warning restore SA1118 // Parameter should not span multiple lines
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
            }");
#pragma warning restore SA1118 // Parameter should not span multiple lines
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
            ]");
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Validates the results of a request to the swagger.json endpoint.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateEndpointJson()
        {
            await AssertSwaggerJsonMatchesExpected(
                "$.paths./weather",
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
             }");
#pragma warning restore SA1118 // Parameter should not span multiple lines
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
            }");
#pragma warning restore SA1118 // Parameter should not span multiple lines

        }

        /// <summary>
        /// Validates that all non prod envs and prod envs are listed in the servers section.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateAllEnvs()
        {
            await AssertSwaggerJsonMatchesExpected(
                "$.servers",
#pragma warning disable SA1118 // Parameter should not span multiple lines
                @"[
                  {
                    'url': 'http://localhost'
                  },
                  {
                    'url': 'https://apigw-np.humanaedge.com/api/cxp/example'
                  },
                  {
                    'url': 'https://apigw-np.humanaedge.com/api/cxp/example-sit'
                  },
                  {
                    'url': 'https://apigw-np.humanaedge.com/api/cxp/example-uat'
                  },
                  {
                    'url': 'https://apigw.humanaedge.com/api/cxp/example'
                  }
                ]");
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        private async Task AssertSwaggerJsonMatchesExpected(string actualSwaggerJpath, string expectedJson)
        {
            var actualValidateResult = await GetSwaggerJObject(actualSwaggerJpath);
            actualValidateResult.Should().BeEquivalentTo(expectedJson);
        }

        private async Task<JToken> GetSwaggerJObject(string jpathSelector)
        {
            var client = _factory.CreateDefaultClient();
            var swaggerResponseMessage = await client.GetAsync(SwaggerPath);
            var swaggerJObject = JObject.Parse(await swaggerResponseMessage.Content.ReadAsStringAsync());
            var actualValidateResult = swaggerJObject.SelectToken(jpathSelector);
            return actualValidateResult;
        }
    }
}