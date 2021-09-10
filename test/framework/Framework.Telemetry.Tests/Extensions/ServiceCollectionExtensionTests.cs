using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Telemetry.Sinks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Telemetry.Tests.Extensions
{
    /// <summary>
    /// Unit tests for the <see cref="ServiceCollectionExtensions" /> class.
    /// </summary>
    public class ServiceCollectionExtensionTests : BaseTests
    {
        /// <summary>
        /// Verifies the behavior of the <see cref="ServiceCollectionExtensions.AddApplicationTelemetry" /> method.
        /// </summary>
        [Fact]
        public void AddApplicationTelemetryTest()
        {
            // arrange
            var serviceCollection = new ServiceCollection();

            // act
            serviceCollection.AddApplicationTelemetry();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var telemetry = serviceProvider.GetServices<LoggerSink>();

            // assert
            Assert.IsType<LoggerSink[]>(telemetry);
        }
    }
}