using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.Alerting;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests.Alerting
{
    /// <summary>
    /// Unit tests for <see cref="HttpAlertingService" />.
    /// </summary>
    public class HttpAlertingServiceTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly HttpAlertingService _httpAlertingService;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public HttpAlertingServiceTests()
        {
            _httpAlertingService = new HttpAlertingService();
        }

        /// <summary>
        /// Verifies the behavior of
        /// <see cref="HttpAlertingService.IsHttpAlert(BaseRestResponse, AlertCondition{BaseRestResponse},AlertCondition{BaseRestResponse})" />.
        /// </summary>
        /// <param name="clientCondition">The client condition.</param>
        /// <param name="requestCondition">The REST request condition.</param>
        /// <param name="result">The value IsHttpAlert should return.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        public async Task IsHttpAlert_ConditionTrue_ReturnsBool(bool clientCondition, bool requestCondition, bool result)
        {
            // arrange
            var fakeRestResponse = await CreateRestResponse<RestResponse>();

            var clientAlertCondition = new AlertCondition<BaseRestResponse>
            {
                Condition = _ => clientCondition,
                ThrowOnFailure = true
            };
            var requestAlertCondition = new AlertCondition<BaseRestResponse>
            {
                Condition = _ => requestCondition,
                ThrowOnFailure = true
            };

            // act
            var actual = _httpAlertingService.IsHttpAlert(fakeRestResponse, requestAlertCondition, clientAlertCondition);

            // assert
            actual.Should().Be(result);
        }

        /// <summary>
        /// Verifies the behavior of
        /// <see cref="HttpAlertingService.IsHttpAlert(BaseRestResponse, AlertCondition{BaseRestResponse},AlertCondition{BaseRestResponse})" />.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task IsHttpAlert_NullConditions_ReturnsFalse()
        {
            // arrange
            var fakeRestResponse = await CreateRestResponse<RestResponse>();
            var clientAlertCondition = new AlertCondition<BaseRestResponse>();
            var requestAlertCondition = new AlertCondition<BaseRestResponse>();

            // act
            var actual = _httpAlertingService.IsHttpAlert(fakeRestResponse, requestAlertCondition, clientAlertCondition);

            // assert
            actual.Should().BeFalse();
        }

        /// <summary>
        /// Verifies the behavior of
        /// <see cref="HttpAlertingService.ThrowIfAlertedAndNeedingException(AlertCondition{BaseRestResponse},AlertCondition{BaseRestResponse})" />.
        /// </summary>
        /// <param name="clientThrow">The bool to throw on failure for the client.</param>
        /// <param name="requestThrow">The bool to throw on failure for the request.</param>
        /// <param name="result">The value IsHttpAlert should return.</param>
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        public void ThrowIfAlertedAndNeedingException_ThrowsAlertConditionMetException(bool clientThrow, bool requestThrow, bool result)
        {
            // arrange
            var clientAlertCondition = new AlertCondition<BaseRestResponse>
            {
                Condition = _ => true,
                ThrowOnFailure = clientThrow
            };
            var requestAlertCondition = new AlertCondition<BaseRestResponse>
            {
                Condition = _ => true,
                ThrowOnFailure = requestThrow
            };

            // assert
            Action act = () => _httpAlertingService.ThrowIfAlertedAndNeedingException(requestAlertCondition, clientAlertCondition);

            // assert
            if (result)
            {
                act.Should().Throw<AlertConditionMetException>();
            }
            else
            {
                act.Should().NotThrow<AlertConditionMetException>();
            }
        }

        private static async Task<RestResponse> CreateRestResponse<TResponse>()
            where TResponse : class
        {
            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            var responseBytes =
                fakeHttpResponseMessage.Content != null
                    ? await fakeHttpResponseMessage.Content.ReadAsByteArrayAsync()
                    : Array.Empty<byte>();
            var deserializer = new TestRestResponseDeserializer(_ => null, responseBytes);
            return new RestResponse(
                fakeHttpResponseMessage.IsSuccessStatusCode,
                deserializer,
                fakeHttpResponseMessage.StatusCode,
                fakeHttpResponseMessage.Headers.Location);
        }
    }
}