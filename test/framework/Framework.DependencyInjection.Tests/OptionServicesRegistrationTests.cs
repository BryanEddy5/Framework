using System.Linq;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.DependencyInjection.Tests.Stubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="OptionServicesRegistration" /> class.
    /// </summary>
    public class OptionServicesRegistrationTests : BaseTests
    {
        /// <summary>
        /// Mock of the configuration file.
        /// </summary>
        private readonly Mock<IConfiguration> _mockConfiguration;

        /// <summary>
        /// Mock of dependency.
        /// </summary>
        private readonly Mock<IDependency> _mockDependency;

        /// <summary>
        /// Common test setup code.
        /// </summary>
        public OptionServicesRegistrationTests()
        {
            _mockDependency = Moq.Create<IDependency>();
            _mockConfiguration = Moq.Create<IConfiguration>();
        }

        /// <summary>
        /// Verifies resolution of an enumeration of services.
        /// </summary>
        [Fact]
        public void RegistrationOfDiOptionsAttributeTest()
        {
            // arrange
            var services = new ServiceCollection();
            var fakeFoo = FakeData.Create<FooClientOptions>();
            _mockConfiguration.SetupGetValue("FooClientOptions").ReturnsValue(fakeFoo);
            services.AddSingleton(_mockDependency.Object);

            var assembly = typeof(OptionServicesRegistrationTests).Assembly;

            services.AddOptionServices(
                _mockConfiguration.Object,
                new[] { assembly });

            // act
            var registeredService = services.Where(x => x.ServiceType.ToString().Contains(nameof(FooClientOptions)))
                .OrderBy(x => x.ServiceType.ToString())
                .ToArray();

            // assert
            registeredService.Should().NotBeEmpty();
        }
    }
}