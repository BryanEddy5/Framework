using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Encryption;
using HumanaEdge.Webcore.Core.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Encryption.Tests
{
    /// <summary>
    /// Integration tests for the KmsEncryptionService.
    /// </summary>
    public class KmsEncryptionServiceIntegrationTests : BaseTests
    {
        private readonly EncryptionServiceOptions _optionsValue;

        /// <summary>
        /// Ctor.
        /// </summary>
        public KmsEncryptionServiceIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            _optionsValue = config.GetSection(nameof(EncryptionServiceOptions)).Get<EncryptionServiceOptions>();
        }

        /// <summary>
        /// Verifies behavior for the encryption happy path.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact(Skip = "Need KMS Setup. See GCPOPS-404")]
        public async Task EncryptSymmetric_ReturnsEncryptedString()
        {
            // arrange
            var testMessage = FakeData.Create<string>();
            var service = new KmsEncryptionService(GetOptionsMock(), new KeyManagementServiceClientFactory());

            // act
            var encryptedString = await service.EncryptSymmetric(testMessage);

            // assert
            encryptedString.Should().NotBe(testMessage);
        }

        /// <summary>
        /// Verifies behavior for the decryption happy path.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact(Skip = "Need KMS Setup. See GCPOPS-404")]
        public async Task DecryptSymmetric_ReturnsEncryptedString()
        {
            // arrange
            var testMessage = FakeData.Create<string>();
            var service = new KmsEncryptionService(GetOptionsMock(), new KeyManagementServiceClientFactory());

            // act
            var encryptedString = await service.DecryptSymmetric(testMessage);

            // assert
            encryptedString.Should().NotBe(testMessage);
        }

        /// <summary>
        /// Verifies that the encryption/decryption of a string returns it to it's original form.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact(Skip = "Need KMS Setup. See GCPOPS-404")]
        public async Task EncryptSymmetricDecryptSymmetric_ReturnsSameString()
        {
            // arrange
            var testMessage = FakeData.Create<string>();
            var service = new KmsEncryptionService(GetOptionsMock(), new KeyManagementServiceClientFactory());

            // act
            var encryptedString = await service.EncryptSymmetric(testMessage);

            // assert
            encryptedString.Should().NotBe(testMessage);

            // act
            var unencryptedMessage = await service.DecryptSymmetric(encryptedString);

            // assert
            unencryptedMessage.Should().Be(testMessage);
        }

        private IOptionsMonitor<EncryptionServiceOptions> GetOptionsMock(bool setup = true)
        {
            var mockOptions = Moq.Create<IOptionsMonitor<EncryptionServiceOptions>>();
            if (setup)
            {
                mockOptions.Setup(m => m.CurrentValue).Returns(_optionsValue);
            }

            return mockOptions.Object;
        }
    }
}