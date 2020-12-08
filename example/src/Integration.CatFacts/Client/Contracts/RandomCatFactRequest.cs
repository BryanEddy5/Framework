namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Client.Contracts
{
    /// <summary>
    /// The request contract from Cat Facts.
    /// </summary>
    public sealed class RandomCatFactRequest
    {
        /// <summary>
        /// The source of the service.
        /// </summary>
        public string? Source { get; }

        /// <summary>
        /// The text of the cat facts.
        /// </summary>
        public string? Text { get; }
    }
}