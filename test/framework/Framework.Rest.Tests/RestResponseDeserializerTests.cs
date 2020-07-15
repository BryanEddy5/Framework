using System;
using System.Net.Http.Headers;
using System.Text;
using AutoFixture;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.Tests.Stubs;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests
{
    /// <summary>
    /// Unit tests for <see cref="RestResponseDeserializer" /> class.
    /// </summary>
    public class RestResponseDeserializerTests : BaseTests
    {
        /// <summary>
        /// System under test.
        /// </summary>
        private RestResponseDeserializer _restResponseDeserializer;

        /// <summary>
        /// Verifies the behavior of <see cref="RestResponseDeserializer.ConvertTo{T}()"/> when an exception is thrown
        /// during deserialization.
        /// </summary>
        [Fact]
        public void ConvertTo_ThrowsException_CannotParse()
        {
            // arrange
            var settings = new RestClientOptions.Builder("https://localhost:5000").Build();
            var mediaTypeFormatters = new[] { new JsonMediaTypeFormatter() };
            var expected = FakeData.Create<Foo>();
            var json = $"{{\"Name\":\"{expected.Name}\", \"Age\":{expected.Age}}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var mediaTypeHeader =
                MediaTypeHeaderValue.Parse($"{MediaType.Json.MimeType}; charset={Encoding.UTF32.WebName}");

            // act
            _restResponseDeserializer = new RestResponseDeserializer(
                mediaTypeFormatters,
                mediaTypeHeader,
                bytes,
                settings);

            // assert
            Assert.Throws<FormatFailedRestException>(() => _restResponseDeserializer.ConvertTo<Bar>());
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestResponseDeserializer.ConvertTo{T}()"/> when no bytes are
        /// present during deserialization.
        /// </summary>
        [Fact]
        public void ConvertTo_ThrowsException_NoBytes()
        {
            // arrange
            var settings = new RestClientOptions.Builder("https://localhost:5000").Build();
            var mediaTypeFormatters = new[] { new JsonMediaTypeFormatter() };
            var bytes = new byte[] { };
            var mediaTypeHeader = new MediaTypeHeaderValue(MediaType.Json.MimeType);

            // act
            _restResponseDeserializer = new RestResponseDeserializer(
                mediaTypeFormatters,
                mediaTypeHeader,
                bytes,
                settings);

            // assert
            Assert.Throws<FormatFailedRestException>(() => _restResponseDeserializer.ConvertTo<Foo>());
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestResponseDeserializer.ConvertTo{T}()"/> when no content
        /// header is included in the response.
        /// </summary>
        [Fact]
        public void ConvertTo_ThrowsException_NoContentTypeHeader()
        {
            // arrange
            var settings = new RestClientOptions.Builder("https://localhost:5000").Build();
            var mediaTypeFormatters = new[] { new JsonMediaTypeFormatter() };
            var expected = FakeData.Create<Foo>();
            var json = $"{{\"Name\":\"{expected.Name}\", \"Age\":{expected.Age}}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            MediaTypeHeaderValue mediaTypeHeader = null;

            // act
            _restResponseDeserializer = new RestResponseDeserializer(
                mediaTypeFormatters,
                mediaTypeHeader,
                bytes,
                settings);

            // assert
            Assert.Throws<FormatFailedRestException>(() => _restResponseDeserializer.ConvertTo<Foo>());
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestResponseDeserializer.ConvertTo{T}()"/> when a <see cref="IMediaTypeFormatter"/>
        /// could not be found for the response's MIME type.
        /// </summary>
        [Fact]
        public void ConvertTo_ThrowsException_NoMatchingFormatter()
        {
            // arrange
            var settings = new RestClientOptions.Builder("https://localhost:5000").Build();
            var mediaTypeFormatters = Array.Empty<IMediaTypeFormatter>();
            var expected = FakeData.Create<Foo>();
            var json = $"{{\"Name\":\"{expected.Name}\", \"Age\":{expected.Age}}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var mediaTypeHeader = new MediaTypeHeaderValue(MediaType.Json.MimeType);

            // act
            _restResponseDeserializer = new RestResponseDeserializer(
                mediaTypeFormatters,
                mediaTypeHeader,
                bytes,
                settings);

            // assert
            Assert.Throws<FormatFailedRestException>(() => _restResponseDeserializer.ConvertTo<Foo>());
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestResponseDeserializer.ConvertTo{T}()"/> when the MIME type
        /// from the response is not supported.
        /// </summary>
        [Fact]
        public void ConvertTo_ThrowsException_NoMatchingFormatter_UnsupportedMimeType()
        {
            // arrange
            var settings = new RestClientOptions.Builder("https://localhost:5000").Build();
            var mediaTypeFormatters = new[] { new JsonMediaTypeFormatter() };
            var expected = FakeData.Create<Foo>();
            var json = $"{{\"Name\":\"{expected.Name}\", \"Age\":{expected.Age}}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var mediaTypeHeader = new MediaTypeHeaderValue("text/xml");

            // act
            _restResponseDeserializer = new RestResponseDeserializer(
                mediaTypeFormatters,
                mediaTypeHeader,
                bytes,
                settings);

            // assert
            Assert.Throws<FormatFailedRestException>(() => _restResponseDeserializer.ConvertTo<Foo>());
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestResponseDeserializer.ConvertTo{T}()"/>.
        /// </summary>
        [Fact]
        public void ConvertTo_SuccessfulDeserialization()
        {
            // arrange
            var settings = new RestClientOptions.Builder("https://localhost:5000").Build();
            var mediaTypeFormatters = new[] { new JsonMediaTypeFormatter() };
            var expected = FakeData.Create<Foo>();
            var json = $"{{\"Name\":\"{expected.Name}\", \"Age\":{expected.Age}}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            var mediaTypeHeader = new MediaTypeHeaderValue(MediaType.Json.MimeType);

            // act
            _restResponseDeserializer = new RestResponseDeserializer(
                mediaTypeFormatters,
                mediaTypeHeader,
                bytes,
                settings);
            var actual = _restResponseDeserializer.ConvertTo<Foo>();

            // assert
            Assert.Equal(expected.Age, actual.Age);
            Assert.Equal(expected.Name, actual.Name);
        }
    }
}