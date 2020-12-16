using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Destructurama;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Example.WebApi;
using HumanaEdge.Webcore.Example.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact.Reader;
using Serilog.Formatting.Json;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// Tests for Framework.Logging package.
    /// </summary>
    public class PiiLoggingTests : BaseTests, IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        /// <summary>
        /// designated ctor.
        /// </summary>
        /// <param name="factory"><see cref="WebApplicationFactory{TEntryPoint}"/>.</param>
        public PiiLoggingTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Test PII masking locally.
        /// </summary>
        [Fact]
        public void EntityLogging_EnsurePiiMasked()
        {
            // arrange
            LogEvent evt = null;

            var log = new LoggerConfiguration()
                .Destructure.UsingAttributes()
                .WriteTo.Debug()
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            var entityResponseContract = new PiiEntityResponseContractExample().GetExamples();

            // act
            log.Information("{@entity}", entityResponseContract);

            var dynamicObj = GetDynamicObjFromJson(evt);
            var entity = dynamicObj!.Properties.entity;

            // assert
            (entity.Ids.MedicareId as string).Should().EndWith("**");
            (entity.LastName as string).Should().Be("***");
            (entity.BirthDate as string).Should().Be("***");
            (entity.Zipcode as string).Should().EndWith("****");
            (entity.Phones[0].PhoneNumber as string).Should().MatchRegex("[\\*]{6}[0-9]{4}");
            (entity.Emails[0].EmailAddress as string).Should().MatchRegex("foo@gmail.com");
            (entity.HomeAddress.AddressLine1 as string).Should().MatchRegex("AddressLine1");
            (entity.HomeAddress.AddressLine2 as string).Should().MatchRegex("AddressLine2");
            (entity.HomeAddress.City as string).Should().MatchRegex("City");
        }

        /// <summary>
        /// Test PII masking when logged from the WebApi.
        /// </summary>
        /// <returns>void.</returns>
        [Fact]
        public async Task CaptureSerilogPiiFromWebApi_EnsureProperlyMasked()
        {
            // Arrange
            var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            var client = _factory.CreateDefaultClient();
            await client.GetAsync("pii");

            // Assert
            var logJson = sw.ToString();
            var logLines = logJson.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None);

            foreach (var line in logLines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                JObject dynamicObj;
                try
                {
                    dynamicObj = JObject.Parse(line);
                }
                catch
                {
                    throw new TestClassException($"Couldn't parse log line from webapi as json: '${line}'");
                }

                if ((string)dynamicObj["severity"] != "ERROR")
                {
                    continue;
                }

                // var dynamicObj = GetDynamicObjFromJson(evt);
                var creditCard = dynamicObj["creditCard"].ToObject<PiiController.CreditCard>();

                creditCard.DefaultMasked.Should().Be("***");
                creditCard.CustomMasked.Should().Be("REMOVED");
                creditCard.ShowFirstThreeThenDefaultMasked.Should().Be("123***");
                creditCard.ShowFirstThreeThenDefaultMaskedPreserveLength.Should().Be("123******");
                creditCard.ShowLastThreeThenDefaultMasked.Should().Be("***789");
                creditCard.ShowLastThreeThenDefaultMaskedPreserveLength.Should().Be("******789");
                creditCard.ShowFirstThreeThenCustomMask.Should().Be("123REMOVED");
                creditCard.ShowLastThreeThenCustomMask.Should().Be("REMOVED789");
                creditCard.ShowLastThreeThenCustomMaskPreserveLength.Should().Be("******789");
                creditCard.ShowFirstThreeThenCustomMaskPreserveLength.Should().Be("123******");
                creditCard.ShowFirstAndLastThreeAndDefaultMaskInTheMiddle.Should().Be("123***789");
                creditCard.ShowFirstAndLastThreeAndCustomMaskInTheMiddle.Should().Be("123REMOVED789");
                creditCard.ShowFirstAndLastThreeAndCustomMaskInTheMiddle2.Should().Be("123REMOVED789");
            }
        }

        private static dynamic GetDynamicObjFromJson(LogEvent evt)
        {
            var formatter = new JsonFormatter();
            var output = new StringWriter();
            formatter.Format(evt, output);
            var jsonString = output.ToString();
            dynamic dynamicObj = JsonConvert.DeserializeObject<ExpandoObject>(
                jsonString,
                new ExpandoObjectConverter());
            return dynamicObj;
        }
    }
}