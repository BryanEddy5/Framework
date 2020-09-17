using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Serializer = System.Xml.Serialization.XmlSerializer;

namespace HumanaEdge.Webcore.Core.Common.Serialization
{
    /// <summary>
    ///     A utility class for manager <see cref="Serializer" /> instances.
    /// </summary>
    public static class XmlSerializer
    {
        /// <summary>
        ///     A cache for pre-constructed XML serializer instances.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Serializer> XmlSerializerInstances =
            new ConcurrentDictionary<Type, Serializer>();

        /// <summary>
        ///     Pre-initialized namespace overrides to force empty namespaces.
        /// </summary>
        private static readonly XmlSerializerNamespaces EmptySerializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });

        /// <summary>
        ///     UTF-8 encoding without a BOM.
        /// </summary>
        private static readonly Encoding _utf8NoBom = new UTF8Encoding(false);

        /// <summary>
        ///     Serializes an object to a UTF-8 string.
        /// </summary>
        /// <typeparam name="T">The type of the object being serialized.</typeparam>
        /// <param name="obj">The instance of the object being serialized.</param>
        /// <returns>The serialized version of the object, as a UTF-8 string.</returns>
        public static string Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetString(SerializeBytes(obj));
        }

        /// <summary>
        ///     Serializes an object to binary content.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized object, as binary.</returns>
        public static byte[] SerializeBytes<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(obj, stream);

                return stream.ToArray();
            }
        }

        /// <summary>
        ///     Serializes the given object to the given stream.
        /// </summary>
        /// <typeparam name="T">The type of the object being serialized.</typeparam>
        /// <param name="obj">The instance of the object being serialized.</param>
        /// <param name="stream">The stream to serialize the object to.</param>
        public static void Serialize<T>(T obj, Stream stream)
        {
            var serializer = XmlSerializerInstances.GetOrAdd(typeof(T), CreateSerializer);
            var xmlWriterSettings = new XmlWriterSettings
            {
                Encoding = _utf8NoBom,
                NewLineHandling = NewLineHandling.None,
                Indent = false
            };
            using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, obj, EmptySerializerNamespaces);
            }
        }

        /// <summary>
        ///     Deserializes an object from a given stream.
        /// </summary>
        /// <typeparam name="T">The type of the object being de-serialized.</typeparam>
        /// <param name="stream">The stream being deserialized from.</param>
        /// <returns>The object instance.</returns>
        public static T Deserialize<T>(Stream stream)
        {
            var serializer = XmlSerializerInstances.GetOrAdd(typeof(T), CreateSerializer);
            return (T)serializer.Deserialize(stream);
        }

        /// <summary>
        ///     Deserializes an object from a given string.
        /// </summary>
        /// <typeparam name="T">The type of the object being de-serialized.</typeparam>
        /// <param name="xml">The XML being deserialized.</param>
        /// <returns>The object instance.</returns>
        public static T Deserialize<T>(string xml)
        {
            var bytes = Encoding.UTF8.GetBytes(xml);
            return DeserializeBytes<T>(bytes);
        }

        /// <summary>
        ///     Deserializes an object from the given binary XML content.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="bytes">The binary content, representing XML.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeBytes<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Deserialize<T>(stream);
            }
        }

        /// <summary>
        ///     Creates an instance of <see cref="Serializer" /> for the given type.
        /// </summary>
        /// <param name="t">The type being serialized.</param>
        /// <returns>The serializer handler for that type.</returns>
        private static Serializer CreateSerializer(Type t)
        {
            return new Serializer(t);
        }
    }
}