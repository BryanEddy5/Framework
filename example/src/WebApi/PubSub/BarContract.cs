namespace HumanaEdge.Webcore.Example.WebApi.PubSub
{
    /// <summary>
    /// A subscription contract the mirrors the shape of the expected Published Topic Message in GCP.
    /// </summary>
    public class BarContract
    {
        /// <summary>
        /// Some field in the contract.
        /// </summary>
        public FooContract? Foo { get; set; }
    }
}