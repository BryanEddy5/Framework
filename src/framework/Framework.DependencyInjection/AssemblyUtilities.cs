using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace HumanaEdge.Webcore.Framework.DependencyInjection
{
    /// <summary>
    ///     Utility methods for working with assemblies.
    /// </summary>
    public static class AssemblyUtilities
    {
        /// <summary>
        ///     Loads all assemblies, starting with the seed assembly, using a recursive algorithm that visits declared
        ///     dependencies.
        /// </summary>
        /// <param name="seed">The seed assembly to start the recursive search from.</param>
        /// <param name="filter">
        ///     Optional. Filters which dependencies are scanned and included in the result set. Should return
        ///     'true' if the assembly should be included.
        /// </param>
        /// <returns>An array of loaded assemblies.</returns>
        public static Assembly[] GetAssemblyAndDependencies(this Assembly seed, Func<AssemblyName, bool>? filter = null)
        {
            if (filter == null)
            {
                filter = asm => true;
            }

            var dependencyContext = DependencyContext.Load(seed);
            return dependencyContext.RuntimeLibraries.SelectMany(lib => lib.GetDefaultAssemblyNames(dependencyContext))
                .Where(assemblyName => filter == null || filter(assemblyName))
                .Select(Assembly.Load)
                .Reverse() // put the seed assembly at the end
                .ToArray();
        }
    }
}