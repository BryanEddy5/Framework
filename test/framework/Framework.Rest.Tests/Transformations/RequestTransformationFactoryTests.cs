using System;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.Transformations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests.Transformations
{
    /// <summary>
    /// Unit tests for <see cref="RequestTransformationFactory" />.
    /// </summary>
    public class RequestTransformationFactoryTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly RequestTransformationFactory _requestTransformationFactory;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public RequestTransformationFactoryTests()
        {
            var mockAccessTokenCache = Moq.Create<IAccessTokenCacheService>(MockBehavior.Loose);
            var mockMediaTypeFormatter = Moq.Create<IMediaTypeFormatter>(MockBehavior.Loose);
            var mockHttpContextAccessor = Moq.Create<IHttpContextAccessor>(MockBehavior.Loose);

            _requestTransformationFactory = new RequestTransformationFactory(
                mockAccessTokenCache.Object,
                mockHttpContextAccessor.Object,
                new[] { mockMediaTypeFormatter.Object });
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RequestTransformationFactory.Create(RestClientOptions)" /> with all
        /// <see cref="RestClientOptions" /> containing values.
        /// </summary>
        [Fact]
        public void Create_ReturnsRequestTransformationService()
        {
            // arrange
            var options = new RestClientOptions.Builder("https://localhost:5000")
                .ConfigureHeader("Foo", "Bar")
                .ConfigureTimeout(TimeSpan.FromSeconds(7))
                .ConfigureJsonFormatting(StandardSerializerConfiguration.Settings)
                .ConfigureMiddleware(
                    r =>
                    {
                        r.Headers["Id"] = Guid.NewGuid().ToString();
                        return r;
                    })
                .ConfigureMiddlewareAsync(
                    async (restRequest, cancellationToken) =>
                    {
                        restRequest.UseHeader("x-jeremy-is", "awesomely-asynchronous");
                        await Task.Delay(1000, cancellationToken);
                        return restRequest;
                    })
                .ConfigureAlertCondition(CommonRestAlertConditions.None())
                .Build();

            // act
            var actual = _requestTransformationFactory.Create(options);

            // assert
            Assert.IsType<RequestTransformationService>(actual);
        }
    }
}