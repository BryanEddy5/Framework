namespace HumanaEdge.Webcore.Example.WebApi.Contracts
{
    /// <summary>
    /// A Complex type to be thrown with an exception.
    /// </summary>
    public class ComplexExceptionMessage
    {
        /// <summary>
        /// Some nested property.
        /// </summary>
        public FooClass? Foo { get; set; }

        /// <summary>
        /// A top level property.
        /// </summary>
        public string? Bar { get; set; }

        /// <summary>
        /// The class for that nested property.
        /// </summary>
        public class FooClass
        {
            /// <summary>
            /// The property.
            /// </summary>
            public string? Bar { get; set; }
        }
    }
}