namespace HumanaEdge.Webcore.Example.Models.Immutable
{
    /// <summary>
    /// A domain model representing a cat fact.
    /// </summary>
    public sealed class CatFact
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="text">The text of the cat fact.</param>
        public CatFact(string text)
        {
            Text = text;
        }

        /// <summary>
        /// The text of the cat fact.
        /// </summary>
        public string Text { get; }
    }
}