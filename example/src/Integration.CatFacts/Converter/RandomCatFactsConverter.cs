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
        public static CatFact ToCatFact(this RandomCatFactsResponse randomCatFactsResponse) =>
            new CatFact(randomCatFactsResponse.Text !);
    }
}