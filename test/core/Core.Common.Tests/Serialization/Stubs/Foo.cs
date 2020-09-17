using System.Collections.Generic;
using System.Xml.Serialization;

namespace HumanaEdge.Webcore.Core.Common.Tests.Serialization.Stubs
{
    /// <summary>
    ///     A basic XML-serializable class.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    [XmlRoot("Foo")]
    public sealed class Foo
    {
        /// <summary>
        ///     A single element.
        /// </summary>
        [XmlElement("One")]
        public int One { get; set; }

        /// <summary>
        ///     An array element.
        /// </summary>
        [XmlArray("Names")]
        [XmlArrayItem("Name")]
        public List<string> Names { get; set; }
    }
}