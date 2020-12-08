namespace HumanaEdge.Webcore.Example.WebApi.Contracts
{
    /// <summary>
    /// A response contract for a random cat fact.
    /// </summary>
    public class RandomCatFactResponse
    {
        /// <summary>
        /// The text of the cat fact.
        /// </summary>
        public string? Text { get; set; }
    }
}