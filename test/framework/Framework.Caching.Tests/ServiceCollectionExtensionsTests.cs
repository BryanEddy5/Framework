using System;
using System.Collections.Generic;
using HumanaEdge.Webcore.Core.Caching.Extensions;
using HumanaEdge.Webcore.Core.Caching.Options;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Caching.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Caching.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="ServiceCollectionExtensions" /> class.
    /// </summary>
    public class ServiceCollectionExtensionsTests : BaseTests
    {
        /// <summary>
        /// Verifies the behavior of the
        /// <see cref="ServiceCollectionExtensions.AddDistributedCache" /> when the environment is set to Development.
        /// </summary>
        [Fact]
        public void AddDistributedCacheDevelopmentTest()
        {
            // arrange
            var root = Setup(ServiceCollectionExtensions.DevelopmentEnvironment);

            // act
            var serviceProvider = new ServiceCollection().AddDistributedCache(root.GetSection(nameof(CacheOptions)))
                .BuildServiceProvider();
            var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
            var certificateAuthorityService = serviceProvider.GetRequiredService<ICertificateAuthorityService>();
            var certificateValidationFactory = serviceProvider.GetRequiredService<ICertificateValidationFactory>();

            // assert
            Assert.IsType<MemoryDistributedCache>(distributedCache);
            Assert.IsType<CertificateAuthorityService>(certificateAuthorityService);
            Assert.IsType<CertificateValidationFactory>(certificateValidationFactory);
        }

        /// <summary>
        /// Verifies the behavior of the
        /// <see cref="ServiceCollectionExtensions.AddDistributedCache" /> when the environment is NOT set to Development.
        /// </summary>
        /// <param name="environment">The hosting environment.</param>
        [Theory]
        [InlineData("Production")]
        [InlineData("test")]
        [InlineData("")]
        [InlineData(null)]
        public void AddDistributedCacheNotDevelopmentTest(string environment)
        {
            // arrange
            var root = Setup(environment);

            // act
            var serviceProvider = new ServiceCollection().AddDistributedCache(root.GetSection(nameof(CacheOptions)))
                .BuildServiceProvider();
            var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
            var certificateAuthorityService = serviceProvider.GetRequiredService<ICertificateAuthorityService>();
            var certificateValidationFactory = serviceProvider.GetRequiredService<ICertificateValidationFactory>();

            // assert
            Assert.IsType<RedisCache>(distributedCache);
            Assert.IsType<CertificateAuthorityService>(certificateAuthorityService);
            Assert.IsType<CertificateValidationFactory>(certificateValidationFactory);
        }

        private IConfigurationRoot Setup(string environment)
        {
            var builder = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>()
                {
                    { "CacheOptions:ConnectionString", "0.0.0.0:6379" },
                });

            Environment.SetEnvironmentVariable(
                ServiceCollectionExtensions.EnvironmentKey,
                environment);

            return builder.Build();
        }
    }
}