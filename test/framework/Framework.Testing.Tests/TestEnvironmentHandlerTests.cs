using FluentAssertions;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Testing.EnvironmentSetup;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Testing.Tests
{
    /// <summary>
    /// Unit tests for <see cref="TestEnvironmentHandler"/>.
    /// </summary>
    public class TestEnvironmentHandlerTests : BaseTests
    {
        /// <summary>
        /// Verifies the behavior of <see cref="TestEnvironmentHandler.GetEnvironment"/> when the test environment variable is not set.
        /// </summary>
        [Fact]
        public void GetEnvironmentVariableNotSet()
        {
            // arrange act
            var actual = TestEnvironmentHandler.GetEnvironment;

            // assert
            actual.Should().BeEquivalentTo("DEV");
        }
    }
}