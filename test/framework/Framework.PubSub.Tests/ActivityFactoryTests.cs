using System.Collections.Generic;
using FluentAssertions;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.TraceContext;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests
{
    /// <summary>
    /// Unit tests for <see cref="ActivityFactory"/>.
    /// </summary>
    public class ActivityFactoryTests : BaseTests
    {
        /// <summary>
        /// Verifies the behavior of <see cref="ActivityFactory.Create"/>.
        /// </summary>
        [Fact]
        public void SetActivityTrace()
        {
            // arrange
            var fakeTraceId = "981c33a8aa41cb41b787fa191fed4ea5";
            var fakeSpanId = "38d5582ded08314a";
            var pubsubMessage = new PubsubMessage { Attributes = { GetAttributes(fakeTraceId, fakeSpanId) } };
            var activityFactor = new ActivityFactory();

            // act
            var activity = activityFactor.Create(pubsubMessage);

            // assert
            activity.TraceId.ToString().Should().BeEquivalentTo(fakeTraceId);
            activity.ParentSpanId.ToString().Should().BeEquivalentTo(fakeSpanId);
            activity.SpanId.ToString().Should().NotBeEquivalentTo(fakeSpanId);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ActivityFactory.Create"/> when the traceId and spanId are
        /// in the incorrect format.
        /// </summary>
        /// <param name="fakeTraceId">The fake trace id.</param>
        /// <param name="fakeSpanId">The fake span id.</param>
        [InlineData("test", "test")]
        [InlineData("981c33a8aa41cb41b787fa191fed4ea5", "test")]
        [InlineData("981c33a8aa41cb41b787fa191fed4ea5", "")]
        [InlineData("test", "38d5582ded08314a")]
        [InlineData("", "38d5582ded08314a")]
        [Theory]
        public void SetActivityTrace_IncorrectFormat(string fakeTraceId, string fakeSpanId)
        {
            // arrange
            var pubsubMessage = new PubsubMessage { Attributes = { GetAttributes(fakeTraceId, fakeSpanId) } };
            var activityFactor = new ActivityFactory();

            // act
            var activity = activityFactor.Create(pubsubMessage);

            // assert
            activity.TraceId.ToString().Should().NotBeEquivalentTo(fakeTraceId);
            activity.ParentSpanId.ToString().Should().NotBeEquivalentTo(fakeSpanId);
        }

        private IDictionary<string, string> GetAttributes(
            string traceId,
            string spanId)
        {
            return new Dictionary<string, string>
            {
                { TracingKeys.TraceId, traceId },
                { TracingKeys.SpanId, spanId },
            };
        }
    }
}