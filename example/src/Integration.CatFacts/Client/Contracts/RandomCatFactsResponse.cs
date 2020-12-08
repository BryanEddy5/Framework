namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Client.Contracts
{
    /// <summary>
    /// The response contract from Cat Facts.
    /// </summary>
    internal sealed class RandomCatFactsResponse
    {
        /// <summary>
        /// The status of the current service.
        /// </summary>
        public CatFactStatus? Status { get; set; }

        /// <summary>
        /// The source of the service.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// The text of the cat facts.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// The current status of the cat fact.
        /// </summary>
        public class CatFactStatus
        {
            /// <summary>
            /// Indicates a verified session.
            /// </summary>
            public string? Verified { get; set; }

            /// <summary>
            /// The number of sent items.
            /// </summary>
            public string? SentCount { get; set; }
        }
    }
}