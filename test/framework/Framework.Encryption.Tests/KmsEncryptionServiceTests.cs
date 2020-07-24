using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Google.Cloud.Kms.V1;
using Google.Protobuf;
using HumanaEdge.Webcore.Core.Encryption;
using HumanaEdge.Webcore.Core.Testing;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Encryption.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="KmsEncryptionService "/> class.
    /// </summary>
    public class KmsEncryptionServiceTests : BaseTests
    {
        private readonly Mock<IKeyManagementServiceClientFactory> _mockKmsClientFactory;

        private readonly Mock<KeyManagementServiceClient> _mockKmsClient;

        private readonly EncryptionServiceOptions _optionsValue;

        /// <summary>
        /// Ctor.
        /// </summary>
        public KmsEncryptionServiceTests()
        {
            _mockKmsClientFactory = Moq.Create<IKeyManagementServiceClientFactory>();
            _mockKmsClient = Moq.Create<KeyManagementServiceClient>();
            _mockKmsClientFactory.Setup(k => k.CreateAsync()).ReturnsAsync(_mockKmsClient.Object);
            _optionsValue = FakeData.Create<EncryptionServiceOptions>();
        }

        /// <summary>
        /// Verifies behavior for the encryption happy path.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task EncryptSymmetric_ReturnsEncryptedString()
        {
            // arrange
            var responseCiphertext = FakeData.Create<string>();
            var encryptResponse = FakeData.Create<EncryptResponse>();
            encryptResponse.Ciphertext = ByteString.CopyFromUtf8(responseCiphertext);
            var testMessage = FakeData.Create<string>();

            _mockKmsClient.Setup(m =>
                m.EncryptAsync(KmsEncryptionService.GetCryptoKeyName(_optionsValue), ByteString.CopyFromUtf8(testMessage), null))
                .ReturnsAsync(encryptResponse);

            var service = new KmsEncryptionService(GetOptionsMock(), _mockKmsClientFactory.Object);

            // act
            var encryptedBytes = await service.EncryptSymmetric(testMessage);

            // assert
            encryptedBytes.Should().BeEquivalentTo(responseCiphertext);
        }

        /// <summary>
        /// Verifies that any error from the KMS client factory bubbles up the exception.
        /// </summary>
        [Fact]
        public void EncryptSymmetric_FactoryThrowsException()
        {
            // arrange
            var testMessage = FakeData.Create<string>();

            _mockKmsClientFactory.Setup(k => k.CreateAsync())
                .ThrowsAsync(new Exception());
            var service = new KmsEncryptionService(GetOptionsMock(false), _mockKmsClientFactory.Object);

            // act, assert
            service.Awaiting(s => s.EncryptSymmetric(testMessage)).Should().Throw<Exception>();
        }

        /// <summary>
        /// Verifies that any error from the KMS client bubbles up the exception.
        /// </summary>
        [Fact]
        public void EncryptSymmetric_ClientThrowsException()
        {
            // arrange
            var testMessage = FakeData.Create<string>();

            _mockKmsClient.Setup(m =>
                    m.EncryptAsync(KmsEncryptionService.GetCryptoKeyName(_optionsValue), ByteString.CopyFromUtf8(testMessage), null))
                .ThrowsAsync(new Exception());

            var service = new KmsEncryptionService(GetOptionsMock(), _mockKmsClientFactory.Object);

            // act, assert
            service.Awaiting(s => s.EncryptSymmetric(testMessage)).Should().Throw<Exception>();
        }

        /// <summary>
        /// Verifies behavior for the decryption happy path.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task DecryptSymmetric_ReturnsDecryptedString()
        {
            // arrange
            var responsePlaintext = FakeData.Create<string>();
            var result = FakeData.Create<DecryptResponse>();
            result.Plaintext = ByteString.CopyFromUtf8(responsePlaintext);
            var testString = FakeData.Create<string>();

            _mockKmsClient.Setup(
                    m =>
                        m.DecryptAsync(KmsEncryptionService.GetCryptoKeyName(_optionsValue), ByteString.CopyFromUtf8(testString), null))
                .ReturnsAsync(result);

            var service = new KmsEncryptionService(GetOptionsMock(), _mockKmsClientFactory.Object);

            // act
            var decryptedString = await service.DecryptSymmetric(testString);

            // assert
            responsePlaintext.Should().BeEquivalentTo(decryptedString);
        }

        /// <summary>
        /// Verifies that any error from the KMS client factory bubbles up the exception.
        /// </summary>
        [Fact]
        public void DecryptSymmetric_FactoryThrowsException()
        {
            // arrange
            var testString = FakeData.Create<string>();

            _mockKmsClientFactory.Setup(k => k.CreateAsync())
                .ThrowsAsync(new Exception());

            var service = new KmsEncryptionService(GetOptionsMock(false), _mockKmsClientFactory.Object);

            // act, assert
            service.Awaiting(s => s.DecryptSymmetric(testString)).Should().Throw<Exception>();
        }

        /// <summary>
        /// Verifies that any error from the KMS client bubbles up the exception.
        /// </summary>
        [Fact]
        public void DecryptSymmetric_ClientThrowsException()
        {
            // arrange
            var testString = FakeData.Create<string>();

            _mockKmsClient.Setup(
                    m =>
                        m.DecryptAsync(KmsEncryptionService.GetCryptoKeyName(_optionsValue), ByteString.CopyFromUtf8(testString), null))
                .ThrowsAsync(new Exception());

            var service = new KmsEncryptionService(GetOptionsMock(), _mockKmsClientFactory.Object);

            // act, assert
            service.Awaiting(s => s.DecryptSymmetric(testString)).Should().Throw<Exception>();
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