using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Testing;
using Newtonsoft.Json;
using Polly;
using Xunit;

namespace HumanaEdge.Webcore.Core.Rest.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="RestClientOptions"/>.
    /// </summary>
    public class RestClientOptionsTests : BaseTests
    {
        /// <summary>
        /// Validates the behavior of <see cref="RestClientOptions"/> configure header functionality.<br/>
        /// Ensures the header is added to the default header collection.
        /// </summary>
        [Fact]
        public void ConfigureHeaderTest()
        {
            // arrange
            var uri = new Uri("http://localhost:5000");
            var headerName = FakeData.Create<string>();
            var headerValue = FakeData.Create<string>();

            // act
            var restClientOptions = new RestClientOptions.Builder(uri)
                .ConfigureHeader(headerName, headerValue)
                .Build();

            // assert
            restClientOptions.DefaultHeaders.Count.Should().Be(1);
            restClientOptions.DefaultHeaders[headerName].Should().BeEquivalentTo(headerValue);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestClientOptions"/> configure json serializer settings
        /// functionality.<br/>
        /// Ensures the settings is overwritten with the configuration that is provided.
        /// </summary>
        [Fact]
        public void ConfigureJsonFormattingTest()
        {
            // arrange
            var uri = new Uri("http://localhost:5000");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };

            // act
            var restClientOptions = new RestClientOptions.Builder(uri)
                .ConfigureJsonFormatting(settings)
                .Build();

            // assert
            restClientOptions.JsonSerializerSettings.ReferenceLoopHandling
                .Should().Be(settings.ReferenceLoopHandling);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestClientOptions"/> middleware functionality.<br/>
        /// Ensures that the .ConfigureMiddleware functionality works.
        /// </summary>
        [Fact]
        public void ConfigureMiddlewareTest()
        {
            // arrange
            var uri = new Uri("http://localhost:5000");
            RestClientOptions.RestRequestTransformation fn1 = restRequest =>
            {
                restRequest.UseHeader("foo", "bar");
                return restRequest;
            };

            RestClientOptions.RestRequestTransformation fn2 = restRequest =>
            {
                restRequest.UseHeader("baz", "qux");
                return restRequest;
            };

            // act
            var restClientOptions = new RestClientOptions.Builder(uri)
                .ConfigureMiddleware(fn1)
                .ConfigureMiddleware(fn2)
                .Build();

            // assert
            restClientOptions.DefaultHeaders.Should().BeEmpty();
            restClientOptions.RestRequestMiddlewareAsync.Length.Should().Be(0); // shouldn't be affected!
            restClientOptions.RestRequestMiddleware.Length.Should().Be(2);
            restClientOptions.RestRequestMiddleware[0].Should().Be(fn1);
            restClientOptions.RestRequestMiddleware[1].Should().Be(fn2);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestClientOptions"/> middleware functionality.<br/>
        /// Ensures that the .ConfigureMiddlewareAsync functionality works.
        /// </summary>
        [Fact]
        public void ConfigureMiddlewareAsyncTest()
        {
            // arrange
            var uri = new Uri("http://localhost:5000");
            RestClientOptions.RestRequestTransformationAsync fn1 = (restRequest, cancellationToken) =>
            {
                restRequest.UseHeader("foo", "bar");
                return Task.FromResult(restRequest);
            };

            RestClientOptions.RestRequestTransformationAsync fn2 = (restRequest, cancellationToken) =>
            {
                restRequest.UseHeader("baz", "qux");
                return Task.FromResult(restRequest);
            };

            // act
            var restClientOptions = new RestClientOptions.Builder(uri)
                .ConfigureMiddlewareAsync(fn1)
                .ConfigureMiddlewareAsync(fn2)
                .Build();

            // assert
            restClientOptions.DefaultHeaders.Should().BeEmpty();
            restClientOptions.RestRequestMiddleware.Length.Should().Be(0); // shouldn't be affected!
            restClientOptions.RestRequestMiddlewareAsync.Length.Should().Be(2);
            restClientOptions.RestRequestMiddlewareAsync[0].Should().Be(fn1);
            restClientOptions.RestRequestMiddlewareAsync[1].Should().Be(fn2);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestClientOptions"/> resilience functionality.<br/>
        /// Ensures that the .ConfigureResiliencePolicy functionality works.
        /// </summary>
        [Fact]
        public void ConfigureResiliencePolicyTest()
        {
            // arrange
            var uri = new Uri("http://localhost:5000");
            var policy = Policy.TimeoutAsync<BaseRestResponse>(Timeout.InfiniteTimeSpan);

            // act
            var restClientOptions = new RestClientOptions.Builder(uri)
                .ConfigureResiliencePolicy(policy)
                .Build();

            // assert
            restClientOptions.ResiliencePolicies.Should().Contain(policy);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestClientOptions"/> timeout functionality.<br/>
        /// Ensures that the .ConfigureTimeout functionality works.
        /// </summary>
        [Fact]
        public void ConfigureTimeoutTest()
        {
            // arrange
            var uri = new Uri("http://localhost:5000");
            var timeout = Timeout.InfiniteTimeSpan;

            // act
            var restClientOptions = new RestClientOptions.Builder(uri)
                .ConfigureTimeout(timeout)
                .Build();

            // assert
            restClientOptions.Timeout.Should().Be(timeout);
        }
    }
}