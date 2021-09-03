using System;
using AutoFixture;
using HumanaEdge.Webcore.Core.SecretsManager;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.SecretsManager.Clients;
using HumanaEdge.Webcore.Framework.SecretsManager.Extensions;
using HumanaEdge.Webcore.Framework.SecretsManager.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="ServiceCollectionExtensions" /> class.
    /// </summary>
    public class ServiceCollectionExtensionsTests : BaseTests
    {
        /// <summary>
        /// Verifies the behavior of the
        /// <see cref="ServiceCollectionExtensions.AddSecret{TSecret}(IServiceCollection, System.Action{SecretsOptions})" />
        /// method. All services
        /// necessary to instantiate a <see cref="ISecretsService{TSecret}" /> are registered.
        /// </summary>
        [Fact]
        public void AddRestClientInstantiateFactoryTest()
        {
            // arrange
            Action<SecretsOptions> secretOptions = options => FakeData.Create<SecretsOptions>();
            var serviceProvider = new ServiceCollection().AddSecret<FakeSecret>(secretOptions).BuildServiceProvider();

            // act
            var secretsService = serviceProvider.GetRequiredService<ISecretsService<FakeSecret>>();
            var secretsHandler = serviceProvider.GetRequiredService<ISecretsHandler>();
            var internalSecretsClient = serviceProvider.GetRequiredService<IInternalSecretsClient>();

            // assert
            Assert.IsAssignableFrom<ISecretsService<FakeSecret>>(secretsService);
            Assert.IsAssignableFrom<ISecretsHandler>(secretsHandler);
            Assert.IsAssignableFrom<IInternalSecretsClient>(internalSecretsClient);
        }
    }
}