using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace HumanaEdge.Webcore.Core.Common
{
    /// <summary>
    /// Extension methods for <see cref="IConfiguration" /> class.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets a configuration property which is a list of values. Converts those values to type <see typeparamref="T" />.
        /// Returns an empty list of the property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the data in the list.</typeparam>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="key">The configuration key.</param>
        /// <returns>The list of application configuration.</returns>
        public static IReadOnlyList<T> GetList<T>(this IConfiguration configuration, string key)
            where T : struct
        {
            return configuration.GetSection(key)
                .GetChildren()
                .Select(x => x.Value)
                .Select(t => (T)Convert.ChangeType(t, typeof(T)))
                .ToList();
        }

        /// <summary>
        /// Gets a configuration property which is a list of values. Returns an empty list of the property is not defined.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="key">The configuration key.</param>
        /// <returns>The list of application configuration.</returns>
        public static IReadOnlyList<string> GetList(this IConfiguration configuration, string key)
        {
            return configuration.GetSection(key).GetChildren().Select(x => x.Value).ToList();
        }
    }
}