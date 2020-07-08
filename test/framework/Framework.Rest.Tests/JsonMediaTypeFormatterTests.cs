using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.Tests.Stubs;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests
{
    /// <summary>
    ///     Performs unit tests on <see cref="JsonMediaTypeFormatter" />.
    /// </summary>
    public class JsonMediaTypeFormatterTests : BaseTests
    {
        /// <summary>
        ///     System under test.
        /// </summary>
        private readonly JsonMediaTypeFormatter _jsonMediaTypeFormatter;

        private readonly RestClientOptions _restClientOptions;

        /// <summary>
        ///     Common test setup.
        /// </summary>
        public JsonMediaTypeFormatterTests()
        {
            _restClientOptions = new RestClientOptions.Builder("https://localhost:5000")
                .Build();
            _jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
        }

        /// <summary>
        ///     Verify the behavior of <see cref="JsonMediaTypeFormatter.TryFormat{T}(MediaType, IRestFormattingSettings, T, out HttpContent)"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task TryFormatTest()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var json = $"{{\"age\":{fakeFoo.Age},\"name\":\"{fakeFoo.Name}\"}}";
            var expectedBytes = Encoding.UTF8.GetBytes(json);

            // act
            var didFormat = _jsonMediaTypeFormatter.TryFormat(
                MediaType.Json,
                _restClientOptions,
                fakeFoo,
                out var httpContent);

            // assert
            Assert.True(didFormat);
            Assert.NotNull(httpContent);
            var actualBytes = await httpContent.ReadAsByteArrayAsync();
            Assert.Equal(expectedBytes, actualBytes);
        }

        /// <summary>
        ///     Verify the behavior of <see cref="JsonMediaTypeFormatter.TryFormat{T}(MediaType, IRestFormattingSettings, T, out HttpContent)"/> with
        ///     default encoding.
        /// </summary>
        [Fact]
        public void TryParseTest_DefaultEncoding()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var json = $"{{\"name\":\"{fakeFoo.Name}\", \"age\":{fakeFoo.Age}}}";
            var bytes = Encoding.Default.GetBytes(json);
            var mediaTypeHeader =
                MediaTypeHeaderValue.Parse($"{MediaType.Json.MimeType}; charset={Encoding.Default.WebName}");

            // act
            var didFormat = _jsonMediaTypeFormatter.TryParse<Foo>(
                bytes,
                _restClientOptions,
                mediaTypeHeader,
                out var actualFoo);

            // assert
            Assert.True(didFormat);
            Assert.Equal(fakeFoo.Age, actualFoo.Age);
            Assert.Equal(fakeFoo.Name, actualFoo.Name);
        }

        /// <summary>
        ///     Verify the behavior of <see cref="JsonMediaTypeFormatter.TryParse{T}(byte[], IRestFormattingSettings, MediaTypeHeaderValue, out T)"/>.
        /// </summary>
        [Fact]
        public void TryParseTest_UTF8()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var json = $"{{\"name\":\"{fakeFoo.Name}\", \"age\":{fakeFoo.Age}}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var mediaTypeHeader = new MediaTypeHeaderValue(MediaType.Json.MimeType);

            // act
            var didFormat = _jsonMediaTypeFormatter.TryParse<Foo>(
                bytes,
                _restClientOptions,
                mediaTypeHeader,
                out var actualFoo);

            // assert
            Assert.True(didFormat);
            Assert.Equal(fakeFoo.Age, actualFoo.Age);
            Assert.Equal(fakeFoo.Name, actualFoo.Name);
        }
    }
}