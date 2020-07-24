using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Encryption;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Encryption.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Encryption.Tests
{
    /// <summary>
    /// Tests for the encryption service collection.
    /// </summary>
    public class ServiceCollectionExtensionsTests : BaseTests
    {
        /// <summary>
        /// Tests that encryption services are successfully added to the service container.
        /// </summary>
        [Fact]
        public void AddEncryptionService_Success()
        {
            // arrange
            var encryptionServiceOptions = FakeData.Create<EncryptionServiceOptions>();
            var builder = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    { nameof(EncryptionServiceOptions), JsonSerializer.Serialize(encryptionServiceOptions) }
                });
            var root = builder.Build();
            var provider = new ServiceCollection().AddKmsEncryption(root.GetSection(nameof(EncryptionServiceOptions)))
                .BuildServiceProvider();

            // act
            var actualEncryptionService = provider.GetRequiredService<IEncryptionService>();
            var actualClientFactory = provider.GetRequiredService<IKeyManagementServiceClientFactory>();

            // assert
            actualEncryptionService.Should().BeOfType<KmsEncryptionService>();
            actualClientFactory.Should().BeOfType<KeyManagementServiceClientFactory>();
        }
    }
}