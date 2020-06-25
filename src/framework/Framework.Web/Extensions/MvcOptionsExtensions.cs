using System;
using Microsoft.AspNetCore.Mvc;

namespace HumanaEdge.Webcore.Framework.Web.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="MvcOptions" />.
    /// </summary>
    internal static class MvcOptionsExtensions
    {
        /// <summary>
        ///     Adds a collection of filters into the Mvc pipeline.
        /// </summary>
        /// <param name="mvcOptions">Configuration options for the Mvc pipeline.</param>
        /// <param name="filters">App specific filters to be added to the pipeline.</param>
        internal static void AddFilters(this MvcOptions mvcOptions, Type[] filters)
        {
            foreach (var filter in filters)
            {
                mvcOptions.Filters.Add(filter);
            }
        }
    }
}