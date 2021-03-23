namespace HumanaEdge.Webcore.Example.Models.Immutable
{
#if NET5_0_OR_GREATER
    /// <summary>
    /// Cat facts baby.
    /// </summary>
    public sealed record CatFact
    {
        /// <summary>
        /// The text of the cat fact.
        /// </summary>
        public string? Text { get; init; }
    }
#else
    /// <summary>
    /// Cat facts baby.
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
        public string? Text { get; }
    }
#endif
}