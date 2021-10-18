using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Soap.Resilience;
using HumanaEdge.Webcore.Core.Soap.Tests.Stubs;
using HumanaEdge.Webcore.Core.Testing;
using Microsoft.Extensions.Primitives;
using Polly;
using Xunit;

namespace HumanaEdge.Webcore.Core.Soap.Tests.Client
{
    /// <summary>
    /// Unit tests for <see cref="SoapClientOptions"/>.
    /// </summary>
    public class SoapClientOptionsTests : BaseTests
    {
        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.Build"/>.<br/>
        /// Ensures when only the baseEndpoint is set, that the defaults are leveraged.
        /// </summary>
        [Fact]
        public void NoOptionsSet_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var defaultTimeout = TimeSpan.FromSeconds(8);

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .Build();

            // assert
            builtOptions.BaseEndpoint.Should().Be(fakeBaseEndpoint);
            builtOptions.Headers.Count.Should().Be(0);
            builtOptions.Timeout.Should().Be(defaultTimeout);
            builtOptions.ResiliencePolicies.Length.Should().Be(2);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.ConfigureHeader"/>.<br/>
        /// Ensures when the ConfigureHeader option is used, it works as expected.
        /// </summary>
        [Fact]
        public void ConfigureHeader_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var customHeaderKey = FakeData.Create<string>();
            var customHeaderValue = FakeData.Create<string>();
            var expectedHeaders = new Dictionary<string, StringValues>
            {
                { customHeaderKey, customHeaderValue }
            };

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .ConfigureHeader(customHeaderKey, customHeaderValue)
                .Build();

            // assert
            builtOptions.Headers.Should().BeEquivalentTo(expectedHeaders);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.ConfigureHeader"/>.<br/>
        /// Ensures when the ConfigureHeader option is used multiple times, it works as expected.
        /// </summary>
        [Fact]
        public void MultiConfigureHeader_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var (fakeKey, fakeValue) = (FakeData.Create<string>(), FakeData.Create<string>());
            var fakeValues = Enumerable.Repeat(fakeValue, 6).ToArray();
            var (otherKey, otherValue) = (FakeData.Create<string>(), FakeData.Create<string>());
            var expectedHeaders = new Dictionary<string, StringValues>
            {
                { fakeKey, fakeValues },
                { otherKey, otherValue }
            };

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .ConfigureHeader(fakeKey, fakeValue)
                .ConfigureHeader(fakeKey, fakeValue)
                .ConfigureHeader(fakeKey, fakeValue)
                .ConfigureHeader(fakeKey, fakeValue)
                .ConfigureHeader(fakeKey, fakeValue)
                .ConfigureHeader(fakeKey, fakeValue)
                .ConfigureHeader(otherKey, otherValue)
                .Build();

            // assert
            builtOptions.Headers.Should().BeEquivalentTo(expectedHeaders);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.Build"/>.<br/>
        /// Ensures when a custom timeout is passed, that it works as expected.
        /// </summary>
        [Fact]
        public void ConfigureTimeout_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var customTimeout = TimeSpan.FromDays(25);

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .ConfigureTimeout(customTimeout)
                .Build();

            // assert
            builtOptions.Timeout.Should().Be(customTimeout);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.Build"/>.<br/>
        /// Ensures when the ConfigureTimeout option is used multiple times, it works as expected.
        /// </summary>
        /// <remarks>
        /// This isn't a practical use case, due to the behavior of it (overwrites), but...
        /// </remarks>
        [Fact]
        public void MultiConfigureTimeout_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var ignoredCustomTimeout = TimeSpan.FromDays(25);
            var perceivedCustomTimeout = TimeSpan.FromMilliseconds(FakeData.Create<uint>());

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .ConfigureTimeout(ignoredCustomTimeout)
                .ConfigureTimeout(perceivedCustomTimeout)
                .Build();

            // assert
            builtOptions.Timeout.Should().Be(perceivedCustomTimeout);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.Build"/>.<br/>
        /// Ensures when a custom resiliency policy is passed, that it works as expected.
        /// </summary>
        [Fact]
        public void ConfigureResiliencePolicy_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var customResiliencyPolicy = Policy.NoOpAsync<HttpResponseMessage>();
            var expectedResiliencyPolicies = new List<IAsyncPolicy<HttpResponseMessage>>
            {
                customResiliencyPolicy
            };

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .ConfigureResiliencePolicy(customResiliencyPolicy)
                .Build();

            // assert
            builtOptions.ResiliencePolicies.Should().Contain(expectedResiliencyPolicies);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.Build"/>.<br/>
        /// Ensures when the ConfigureResiliencePolicy is used multiple times,
        /// that it works as expected.
        /// </summary>
        [Fact]
        public void MultiConfigureResiliencePolicy_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var customResiliencyPolicy1 = Policy.NoOpAsync<HttpResponseMessage>();
            var customResiliencyPolicy2 = Policy.NoOpAsync<HttpResponseMessage>();
            var customResiliencyPolicy3 = Policy.NoOpAsync<HttpResponseMessage>();
            var expectedResiliencyPolicies = new List<IAsyncPolicy<HttpResponseMessage>>
            {
                customResiliencyPolicy1, customResiliencyPolicy2, customResiliencyPolicy3
            };

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .ConfigureResiliencePolicy(customResiliencyPolicy1)
                .ConfigureResiliencePolicy(customResiliencyPolicy2)
                .ConfigureResiliencePolicy(customResiliencyPolicy3)
                .Build();

            // assert
            builtOptions.ResiliencePolicies.Should().Contain(expectedResiliencyPolicies);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.Build"/>.<br/>
        /// Ensures when the ConfigureResiliencePolicy is used multiple times,
        /// that it works as expected.
        /// </summary>
        [Fact]
        public void UseDefaultRetryPolicy_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .Build();

            // assert
            builtOptions.ResiliencePolicies.Length.Should().Be(2);
            builtOptions.ResiliencePolicies[1].PolicyKey.Should().StartWith("AsyncRetryPolicy");
        }

        /// <summary>
        /// Verifies the behavior of <see cref="SoapClientOptions.Builder.Build"/>.<br/>
        /// Ensures when all builder options are used, it still builds.
        /// </summary>
        [Fact]
        public void AllOptionsSet_Builds()
        {
            // arrange
            var fakeBaseEndpoint = new Uri("https://humana.com");
            var customTimeout = TimeSpan.FromDays(25);
            var customHeaderKey = FakeData.Create<string>();
            var customHeaderValue = FakeData.Create<string>();
            var expectedHeaders = new Dictionary<string, StringValues>
            {
                { customHeaderKey, customHeaderValue }
            };
            var customResiliencyPolicy = Policy.NoOpAsync<HttpResponseMessage>();
            var expectedResiliencyPolicies = new List<IAsyncPolicy<HttpResponseMessage>>
            {
                customResiliencyPolicy
            };

            // act
            var builtOptions = new SoapClientOptions.Builder(fakeBaseEndpoint)
                .ConfigureTimeout(customTimeout)
                .ConfigureHeader(customHeaderKey, customHeaderValue)
                .ConfigureResiliencePolicy(customResiliencyPolicy)
                .Build();

            // assert
            builtOptions.BaseEndpoint.Should().Be(fakeBaseEndpoint);
            builtOptions.Headers.Should().BeEquivalentTo(expectedHeaders);
            builtOptions.Timeout.Should().Be(customTimeout);
            builtOptions.ResiliencePolicies.Should().Contain(expectedResiliencyPolicies);
        }
    }
}