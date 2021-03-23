using HumanaEdge.Webcore.Example.Integration.CatFacts.Client.Contracts;
using HumanaEdge.Webcore.Example.Models.Immutable;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Converter
{
    /// <summary>
    /// A converter from the integration contracts to a domain model.
    /// </summary>
    internal static class RandomCatFactsConverter
    {
        /// <summary>
        /// Converts <see cref="RandomCatFactsResponse"/> to <see cref="CatFact"/>.
        /// </summary>
        /// <param name="randomCatFactsResponse">The response containing a random cat fact.</param>
        /// <returns>A domain model of a cat fact.</returns>
        public static CatFact ToCatFact(this RandomCatFactsResponse randomCatFactsResponse)
        {
#if NET5_0_OR_GREATER
            return new CatFact { Text = randomCatFactsResponse.Text };
#else
            return new CatFact(randomCatFactsResponse.Text!);
#endif
        }
    }
}