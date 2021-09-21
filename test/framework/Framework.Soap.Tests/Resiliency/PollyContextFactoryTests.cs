using FluentAssertions;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Core.Web.Resiliency;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;
using HumanaEdge.Webcore.Framework.Soap.Tests.Stubs;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Soap.Tests.Resiliency
{
    /// <summary>
    /// Unit tests for <see cref="PollyContextFactory"/>.
    /// </summary>
    public class PollyContextFactoryTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly IPollyContextFactory _pollyContextFactory;

        /// <summary>
        /// Mock of <see cref="ILoggerFactory"/>.
        /// </summary>
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;

        /// <summary>
        /// Common setup code.
        /// </summary>
        public PollyContextFactoryTests()
        {
            _mockLoggerFactory = Moq.Create<ILoggerFactory>();
            _pollyContextFactory = new PollyContextFactory(_mockLoggerFactory.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="IPollyContextFactory.Create"/>.<br/>
        /// Ensures a <see cref="Context"/> is created when the method is executed.
        /// </summary>
        [Fact]
        public void Create_CreatesContext()
        {
            // arrange
            var expected = new Context().WithLogger(_mockLoggerFactory.Object);

            // act
            var actual = _pollyContextFactory.Create();

            // assert
            expected.Should().BeEquivalentTo(actual);
        }
    }
}