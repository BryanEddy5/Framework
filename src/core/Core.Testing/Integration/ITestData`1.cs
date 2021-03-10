namespace HumanaEdge.Webcore.Core.Testing.Integration
{
    /// <summary>
    /// Retrieves test data for integration testing.
    /// </summary>
    /// <typeparam name="TData">The shape of the test data.</typeparam>
    public interface ITestData<out TData>
    {
        /// <summary>
        /// Retrieves the test data.
        /// </summary>
        public TData Get { get; }
    }
}