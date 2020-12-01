using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Destructurama;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Example.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact.Reader;
using Serilog.Formatting.Json;
using Xunit;

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

            // This code does nothing, but makes it a lot easier to
            // examine structured logs in the debugger.
            var sv = (StructureValue)evt.Properties["entity"];
            var props = sv.Properties.ToDictionary(p => p.Name, p => p.Value);

            // This code allows for cleaner assertions.
            var formatter = new JsonFormatter();
            var output = new StringWriter();
            formatter.Format(evt, output);
            var jsonString = output.ToString();
            dynamic dynamicObj = JsonConvert.DeserializeObject<ExpandoObject>(
                jsonString,
                new ExpandoObjectConverter());
            var entity = dynamicObj!.Properties.entity;

            // assert

            // For some reason, I can't use FluentAssertions with dynamics.
            Assert.EndsWith("**", entity.Ids.MedicareId);
            Assert.Equal(entity.LastName, "***");
            Assert.Equal(entity.BirthDate, "***");
            Assert.EndsWith("****", entity.Zipcode);
            Assert.Matches("[\\*]{6}[0-9]{4}", entity.Phones[0].PhoneNumber);
            Assert.Matches("foo@gmail.com", entity.Emails[0].EmailAddress);
            Assert.Matches("AddressLine1", entity.HomeAddress.AddressLine1);
            Assert.Matches("AddressLine2", entity.HomeAddress.AddressLine2);
            Assert.Matches("City", entity.HomeAddress.City);
        }

        /// <summary>
        /// Test PII masking when logged from the WebApi.
        /// </summary>
        /// <returns>void.</returns>
        [Fact]
        public async Task CaptureSerilogPiiFromWebApi_EnsureProperlyMasked()
        {
            // Arrange
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms)
            {
                AutoFlush = true
            };
            Console.SetOut(writer);

            // Act
            var client = _factory.CreateDefaultClient();
            await client.GetAsync("api/v1/pii");

            // Assert
            ms.Position = 0;
            var reader = new LogEventReader(new StreamReader(ms));
            while (reader.TryRead(out var evt))
            {
                if (evt.Level != LogEventLevel.Error)
                {
                    continue;
                }

                // This code does nothing, but makes it a lot easier to
                // examine structured logs in the debugger.
                var sv = (StructureValue)evt.Properties["creditCard"];
                var props = sv.Properties.ToDictionary(p => p.Name, p => p.Value);

                // This code allows for cleaner assertions.
                var formatter = new JsonFormatter();
                var output = new StringWriter();
                formatter.Format(evt, output);
                var jsonString = output.ToString();
                dynamic dynamicObj = JsonConvert.DeserializeObject<ExpandoObject>(
                    jsonString,
                    new ExpandoObjectConverter());
                var creditCard = dynamicObj!.Properties.creditCard;

                // For some reason, I can't use FluentAssertions with dynamics.
                Assert.Equal(creditCard.DefaultMasked, "***");
                Assert.Equal(creditCard.CustomMasked, "REMOVED");
                Assert.Equal(creditCard.ShowFirstThreeThenDefaultMasked, "123***");
                Assert.Equal(creditCard.ShowFirstThreeThenDefaultMaskedPreserveLength, "123******");
                Assert.Equal(creditCard.ShowLastThreeThenDefaultMasked, "***789");
                Assert.Equal(creditCard.ShowLastThreeThenDefaultMaskedPreserveLength, "******789");
                Assert.Equal(creditCard.ShowFirstThreeThenCustomMask, "123REMOVED");
                Assert.Equal(creditCard.ShowLastThreeThenCustomMask, "REMOVED789");
                Assert.Equal(creditCard.ShowLastThreeThenCustomMaskPreserveLength, "******789");
                Assert.Equal(creditCard.ShowFirstThreeThenCustomMaskPreserveLength, "123******");
                Assert.Equal(creditCard.ShowFirstAndLastThreeAndDefaultMaskInTheMiddle, "123***789");
                Assert.Equal(creditCard.ShowFirstAndLastThreeAndCustomMaskInTheMiddle, "123REMOVED789");
                Assert.Equal(creditCard.ShowFirstAndLastThreeAndCustomMaskInTheMiddle2, "123REMOVED789");
            }
        }
    }
}