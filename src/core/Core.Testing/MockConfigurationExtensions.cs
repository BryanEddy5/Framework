using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Language.Flow;

namespace HumanaEdge.Webcore.Core.Testing
{
    /// <summary>
    /// Extension methods for <see cref="Mock{T}" /> to aid with unit testing.
    /// </summary>
    public static class MockConfigurationExtensions
    {
        /// <summary>
        /// Configures a setup for a configuration property access to return a list.
        /// </summary>
        /// <param name="configurationSetup">The in-process setup of the configuration.</param>
        /// <param name="values">The values configured to be returned as a list.</param>
        /// <typeparam name="T">The type of the items in the list. Should be primitives or strings.</typeparam>
        /// <returns>The mock <see cref="IReturnsResult{T}" />.</returns>
        public static IReturnsResult<IConfiguration> ReturnsList<T>(
            this ISetup<IConfiguration, IConfigurationSection> configurationSetup,
            IList<T> values)
        {
            var mockConfigurationSection = new Mock<IConfigurationSection>(MockBehavior.Strict);
            mockConfigurationSection.Setup(s => s.GetChildren())
                .Returns(
                    values.Select(
                        v =>
                        {
                            var mock = new Mock<IConfigurationSection>(MockBehavior.Strict);
                            mock.Setup(m => m.Value).Returns(v!.ToString()!);
                            return mock.Object;
                        }));

            return configurationSetup.Returns(mockConfigurationSection.Object);
        }

        /// <summary>
        /// Configures a setup for a configuration property access to return null, as if the property was not defined.
        /// </summary>
        /// <param name="configurationSetup">The in-process setup of the configuration.</param>
        /// <returns>The mock <see cref="IReturnsResult{T}" />.</returns>
        public static IReturnsResult<IConfiguration> ReturnsNull(
            this ISetup<IConfiguration, IConfigurationSection> configurationSetup)
        {
            var mockConfigurationSection = new Mock<IConfigurationSection>(MockBehavior.Strict);
            mockConfigurationSection.Setup(s => s.Value).Returns((string)null!);
            mockConfigurationSection.Setup(s => s.GetChildren()).Returns(Array.Empty<IConfigurationSection>());

            return configurationSetup.Returns(mockConfigurationSection.Object);
        }

        /// <summary>
        /// Configures a setup for a configuration property access to return a value.
        /// </summary>
        /// <param name="configurationSetup">The in-process setup of the configuration.</param>
        /// <param name="value">The values configured to be returned.</param>
        /// <typeparam name="T">The type of the value returned. Should be primitive or string.</typeparam>
        /// <returns>The mock <see cref="IReturnsResult{T}" />.</returns>
        public static IReturnsResult<IConfiguration> ReturnsValue<T>(
            this ISetup<IConfiguration, IConfigurationSection> configurationSetup,
            T value)
        {
            var mockConfigurationSection = new Mock<IConfigurationSection>(MockBehavior.Strict);

            return configurationSetup.Returns(mockConfigurationSection.Object);
        }

        /// <summary>
        /// Sets up a mock configuration object for an expectation that the "GetList" extension method is run against the
        /// configuration.
        /// </summary>
        /// <param name="mockConfiguration">The configuration mock.</param>
        /// <param name="key">The configuration key being accessed.</param>
        /// <returns>The configuration setup.</returns>
        public static ISetup<IConfiguration, IConfigurationSection> SetupGetList(
            this Mock<IConfiguration> mockConfiguration,
            string key)
        {
            return mockConfiguration.SetupGetValue(key);
        }

        /// <summary>
        /// Sets up a mock configuration object for an expectation that the "GetValue" extension method is run against the
        /// configuration.
        /// </summary>
        /// <param name="mockConfiguration">The configuration mock.</param>
        /// <param name="key">The configuration key being accessed.</param>
        /// <returns>The configuration setup.</returns>
        public static ISetup<IConfiguration, IConfigurationSection> SetupGetValue(
            this Mock<IConfiguration> mockConfiguration,
            string key)
        {
            return mockConfiguration.Setup(cfg => cfg.GetSection(key));
        }
    }
}