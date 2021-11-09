namespace HumanaEdge.Webcore.Core.Soap.Client.Models
{
    /// <summary>
    /// The shape of a SOAP header.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class SoapHeader
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="nameSpace">The header SOAP namespace.</param>
        /// <param name="value">The header value.</param>
        public SoapHeader(string name, string nameSpace, object value)
        {
            Name = name;
            NameSpace = nameSpace;
            Value = value;
        }

        /// <summary>
        /// The header name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The header namespace.
        /// </summary>
        public string NameSpace { get; }

        /// <summary>
        /// The header value.
        /// </summary>
        public object Value { get; }
    }
}