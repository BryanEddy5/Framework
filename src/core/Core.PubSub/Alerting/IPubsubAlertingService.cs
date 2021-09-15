using HumanaEdge.Webcore.Core.Common.Alerting;

namespace HumanaEdge.Webcore.Core.PubSub.Alerting
{
    /// <summary>
    /// A service for tracking telemetry of various pubsub services.
    /// </summary>
    public interface IPubsubAlertingService
    {
        /// <summary>
        /// Inspects a pubsub response to check if it met an alert condition or not.
        /// </summary>
        /// <param name="clientCondition">The client-level alert condition.</param>
        /// <param name="success">Whether or not the publish was a success.</param>
        /// <returns>True if the response met an alert condition, false otherwise.</returns>
        bool IsPubsubAlert(AlertCondition<bool> clientCondition, bool success);

        /// <summary>
        /// Inspects the default and request <see cref="AlertCondition{T}"/>s to see if they required
        /// throwing exceptions upon an alert condition being met. If so, throw accordingly.
        /// </summary>
        /// <param name="clientCondition">The client-level alert condition.</param>
        void ThrowIfAlertedAndNeedingException(AlertCondition<bool> clientCondition);
    }
}