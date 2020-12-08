using HumanaEdge.Webcore.Example.Models.Immutable;
using HumanaEdge.Webcore.Example.WebApi.Contracts;

namespace HumanaEdge.Webcore.Example.WebApi.Converters
{
    /// <summary>
    /// A converter class for random cat fact contracts.
    /// </summary>
    public static class RandomCatFactConverter
    {
        /// <summary>
        /// Converts a <see cref="CatFact"/> to <see cref="RandomCatFactResponse"/>.
        /// </summary>
        /// <param name="catFact">The domain model of a cat fact.</param>
        /// <returns>The response contract for a random cat fact.</returns>
        public static RandomCatFactResponse ToRandomCatFactResponse(this CatFact catFact) =>
            new RandomCatFactResponse { Text = catFact.Text };
    }
}