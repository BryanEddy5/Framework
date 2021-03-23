using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Framework.Rest.Tests.Stubs;
using Newtonsoft.Json;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests
{
    /// <summary>
    /// Unit tests for <see cref="FormUrlEncodedMediaTypeFormatter" /> class.
    /// </summary>
    public class FormUrlEncodedMediaTypeFormatterTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly FormUrlEncodedMediaTypeFormatter _formUrlEncodedMediaTypeFormatter;

        /// <summary>
        /// Settings for the <see cref="FormUrlEncodedMediaTypeFormatter" />.
        /// </summary>
        private readonly IRestFormattingSettings _settings;

        /// <summary>
        /// Common setup code for tests against the <see cref="JsonMediaTypeFormatter" /> class.
        /// </summary>
        public FormUrlEncodedMediaTypeFormatterTests()
        {
            _settings = new RestClientOptions.Builder("http://localhost:5000")
                .ConfigureJsonFormatting(new JsonSerializerSettings())
                .Build();
            _formUrlEncodedMediaTypeFormatter = new FormUrlEncodedMediaTypeFormatter();
        }

        /// <summary>
        /// Verifies the behavior of the <see cref="JsonMediaTypeFormatter.MediaTypes" /> property. Should return only JSON.
        /// </summary>
        [Fact]
        public void MediaTypesTest()
        {
            Assert.Equal(
                new[] { MediaType.FormUrlEncoded },
                _formUrlEncodedMediaTypeFormatter.MediaTypes);
        }

        /// <summary>
        /// Verifies the behavior of the <see cref="FormUrlEncodedMediaTypeFormatter.TryFormat{T}" /> method.
        /// Should convert a class to a dictionary of <see cref="FormUrlEncodedContent" />.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task TryFormatTestAsync()
        {
            // arrange
            var obj = new Foo
            {
                Name = "Bar",
                Age = 12
            };

            var expectedContent = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("Age", "12"), new KeyValuePair<string, string>("Name", "Bar")
                });
            var expectedBytes = await expectedContent.ReadAsByteArrayAsync();

            // act
            var didFormat = _formUrlEncodedMediaTypeFormatter.TryFormat(
                MediaType.FormUrlEncoded,
                _settings,
                obj,
                out var httpContent);

            // assert
            Assert.True(didFormat);
            var binaryContent = httpContent as ByteArrayContent;
            var bytes = await httpContent.ReadAsByteArrayAsync();
            Assert.Equal(expectedContent.Headers.ToString(), httpContent.Headers.ToString());
            Assert.Equal(expectedBytes, bytes);
        }
    }
}