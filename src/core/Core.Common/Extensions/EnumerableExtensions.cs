using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanaEdge.Webcore.Core.Common.Extensions
{
    /// <summary>
    ///     Extension methods for enumerables.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Indexes a collection by a given selector.
        /// </summary>
        /// <param name="values">The collection being indexed.</param>
        /// <param name="indexer">The selector function for the index key.</param>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the collection.</typeparam>
        /// <returns>The dictionary.</returns>
        public static Dictionary<TKey, TValue> IndexBy<TKey, TValue>(
            this IEnumerable<TValue> values,
            Func<TValue, TKey> indexer)
        {
            return values.ToDictionary(indexer, val => val);
        }

        /// <summary>
        ///     An inline foreach method extension of <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable">The collection to iterate through.</param>
        /// <param name="action">A delegate.</param>
        /// <typeparam name="T">The <see cref="Type"/> to be iterated through.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var record in enumerable)
            {
                action(record);
            }
        }
    }
}