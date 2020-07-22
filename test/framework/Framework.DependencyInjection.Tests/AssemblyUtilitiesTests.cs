using System;
using System.Linq;
using System.Reflection;
using HumanaEdge.Webcore.Core.Testing;
using Xunit;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Tests
{
    /// <summary>
    ///     Unit tests for the <see cref="AssemblyUtilities" /> class.
    /// </summary>
    public class AssemblyUtilitiesTests : BaseTests
    {
        /// <summary>
        ///     Verifies the behavior of <see cref="AssemblyUtilities.GetAssemblyAndDependencies" /> to ensure it can recursively
        ///     fetch the full list of assemblies.
        /// </summary>
        [Fact]
        public void GetAssemblyAndDependenciesTest()
        {
            // arrange
            Func<AssemblyName, bool> filter = name =>
                name.Name != null && (name.Name.StartsWith("HumanaEdge") || name.Name.StartsWith("Newtonsoft"));

            // act
            var assemblyList = typeof(AssemblyUtilitiesTests).Assembly.GetAssemblyAndDependencies(filter);

            // assert
            Assert.True(assemblyList.Last().Equals(typeof(AssemblyUtilitiesTests).Assembly));

            // as of the time of writing this test, the dependency path is Framework.DI.Tests => Framework.DI =>
            // Core.Common => Newtonsoft.Json. If at any point the Newtonsoft dependency is changed, we may need to
            // revisit this assert.
            Assert.Contains(assemblyList, assembly => assembly.GetName().Name == "Newtonsoft.Json");

            // this assembly is listed as dependency of this unit test project, but no types from this assembly are ever
            // used. this is so that we have a good test to ensure that our DI framework can pick up registrations from
            // assemblies which _only_ contain DI component implementations are never explicitly touched by the assembly
            // referencing it.
            Assert.Contains(assemblyList, assembly => assembly.GetName().Name == "HumanaEdge.Webcore.Core.Rest");
        }
    }
}