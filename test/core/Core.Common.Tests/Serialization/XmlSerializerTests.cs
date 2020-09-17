using System.Collections.Generic;
using System.IO;
using System.Text;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Common.Tests.Serialization.Stubs;
using HumanaEdge.Webcore.Core.Testing;
using Xunit;

namespace HumanaEdge.Webcore.Core.Common.Tests.Serialization
{
    /// <summary>
    ///     Unit tests for the <see cref="XmlSerializer" /> class.
    /// </summary>
    public class XmlSerializerTests : BaseTests
    {
        /// <summary>
        ///     Verifies the behavior of the <see cref="XmlSerializer.Serialize{T}(T)" /> method.
        /// </summary>
        [Fact]
        public void SerializeStringTest()
        {
            // arrange
            var obj = new Foo
            {
                One = 1,
                Names = new List<string> { "John" }
            };

            var expectedXml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Foo><One>1</One><Names><Name>John</Name></Names></Foo>";

            // act
            var xml = XmlSerializer.Serialize(obj);

            // arrange
            Assert.Equal(expectedXml, xml);
        }

        /// <summary>
        ///     Verifies the behavior of the <see cref="XmlSerializer.Serialize{T}(T, Stream)" /> method.
        /// </summary>
        [Fact]
        public void SerializeStreamTest()
        {
            // arrange
            var obj = new Foo
            {
                One = 1,
                Names = new List<string> { "John" }
            };

            var expectedXml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Foo><One>1</One><Names><Name>John</Name></Names></Foo>";
            var expectedBytes = Encoding.UTF8.GetBytes(expectedXml);

            // act
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                XmlSerializer.Serialize(obj, stream);
                bytes = stream.ToArray();
            }

            // arrange
            Assert.Equal(expectedBytes, bytes);
        }

        /// <summary>
        ///     Verifies the behavior of <see cref="XmlSerializer.SerializeBytes{T}"/> method.
        /// </summary>
        [Fact]
        public void SerializeBytesTest()
        {
            // arrange
            var obj = new Foo
            {
                One = 1,
                Names = new List<string> { "John" }
            };

            var expectedXml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Foo><One>1</One><Names><Name>John</Name></Names></Foo>";
            var expectedBytes = Encoding.UTF8.GetBytes(expectedXml);

            // act
            var bytes = XmlSerializer.SerializeBytes(obj);

            // arrange
            Assert.Equal(expectedBytes, bytes);
        }

        /// <summary>
        ///     Verifies the behavior of <see cref="XmlSerializer.DeserializeBytes{T}"/> method.
        /// </summary>
        [Fact]
        public void DeserializeBytesTest()
        {
            // arrange
            var xml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Foo><One>1</One><Names><Name>John</Name></Names></Foo>";

            var bytes = Encoding.UTF8.GetBytes(xml);

            var expectedObject = new Foo
            {
                One = 1,
                Names = new List<string> { "John" }
            };

            // act
            var actualObject = XmlSerializer.DeserializeBytes<Foo>(bytes);

            // assert
            Assert.Equal(expectedObject, actualObject);
        }

        /// <summary>
        ///     Verifies the behavior of <see cref="XmlSerializer.Deserialize{T}(string)"/> method.
        /// </summary>
        [Fact]
        public void DeserializeStringTest()
        {
            // arrange
            var xml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Foo><One>1</One><Names><Name>John</Name></Names></Foo>";

            var expectedObject = new Foo
            {
                One = 1,
                Names = new List<string> { "John" }
            };

            // act
            var actualObject = XmlSerializer.Deserialize<Foo>(xml);

            // assert
            Assert.Equal(expectedObject, actualObject);
        }

        /// <summary>
        ///     Verifies the behavior of <see cref="XmlSerializer.Deserialize{T}(Stream)"/> method.
        /// </summary>
        [Fact]
        public void DeserializeStreamTest()
        {
            // arrange
            var xml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Foo><One>1</One><Names><Name>John</Name></Names></Foo>";

            var bytes = Encoding.UTF8.GetBytes(xml);

            var expectedObject = new Foo
            {
                One = 1,
                Names = new List<string> { "John" }
            };

            // act
            Foo actualObject;
            using (var stream = new MemoryStream(bytes))
            {
                actualObject = XmlSerializer.Deserialize<Foo>(stream);
            }

            // assert
            Assert.Equal(expectedObject, actualObject);
        }
    }
}