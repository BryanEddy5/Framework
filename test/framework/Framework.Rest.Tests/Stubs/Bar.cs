namespace HumanaEdge.Webcore.Framework.Rest.Tests.Stubs
{
    /// <summary>
    /// Complex object for testing.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public class Bar
    {
        /// <summary>
        /// Nested class for testing.
        /// </summary>
        public Foo Child { get; set; }
    }
}