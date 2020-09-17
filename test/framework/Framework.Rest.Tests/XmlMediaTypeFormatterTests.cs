using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.Tests.Stubs;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests
{
    /// <summary>
    ///     Unit tests for the <see cref="XmlMediaTypeFormatter"/> class.
    /// </summary>
    public class XmlMediaTypeFormatterTests : BaseTests
    {
        /// <summary>
        ///     Settings for the <see cref="XmlMediaTypeFormatter"/>.
        /// </summary>
        private readonly IRestFormattingSettings _settings;

        /// <summary>
        ///     SUT.
        /// </summary>
        private readonly XmlMediaTypeFormatter _xmlMediaTypeFormatter;

        /// <summary>
        ///     Common setup code for tests against the <see cref="XmlMediaTypeFormatter"/> class.
        /// </summary>
        public XmlMediaTypeFormatterTests()
        {
            _settings = new RestClientOptions.Builder("http://localhost:5000").Build();
            _xmlMediaTypeFormatter = new XmlMediaTypeFormatter();
        }

        /// <summary>
        ///     Verifies the behavior of the <see cref="XmlMediaTypeFormatter.MediaTypes"/> property. Should return only XML.
        /// </summary>
        [Fact]
        public void MediaTypesTest()
        {
            Assert.Equal(new[] { MediaType.Xml }, _xmlMediaTypeFormatter.MediaTypes);
        }

        /// <summary>
        ///     Verifies the behavior of the <see cref="XmlMediaTypeFormatter.TryFormat{T}"/> method. Should serialize the object to binary xml content.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task TryFormatTestAsync()
        {
            // arrange
            var obj = FakeData.Create<Foo>();

            // remove any xml reserved characters to ensure the resulting xml is valid.
            obj.Name = new string(obj.Name.Where(char.IsLetter).ToArray());

            var xml = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><Foo><Age>{obj.Age}</Age><Name>{obj.Name}</Name></Foo>";
            var expectedBytes = Encoding.UTF8.GetBytes(xml);

            // act
            var didFormat = _xmlMediaTypeFormatter.TryFormat(
                MediaType.Xml,
                _settings,
                obj,
                out var httpContent);

            // assert
            Assert.True(didFormat);
            var binaryContent = httpContent as ByteArrayContent;
            Assert.NotNull(binaryContent);
            var bytes = await binaryContent.ReadAsByteArrayAsync();
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        /// <summary>
        ///     Verifies the behavior of the <see cref="XmlMediaTypeFormatter.TryParse{T}"/> method. Should deserialize the object from the binary xml content.
        /// </summary>
        [Fact]
        public void TryParseTest()
        {
            // arrange
            var xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Foo><Name>Bar</Name></Foo>";
            var bytes = Encoding.Unicode.GetBytes(xml);

            var mediaTypeHeader =
                MediaTypeHeaderValue.Parse($"{MediaType.Xml.MimeType}; charset={Encoding.Unicode.WebName}");

            var expectedObj = new Foo { Name = "Bar" };

            // act
            var didParse = _xmlMediaTypeFormatter.TryParse<Foo>(bytes, _settings, mediaTypeHeader, out var obj);

            // assert
            Assert.True(didParse);
            Assert.Equal(expectedObj, obj);
        }
    }
}