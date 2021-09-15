using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Exceptions;

namespace HumanaEdge.Webcore.Core.Rest.Alerting
{
    /// <summary>
    /// A service for tracking telemetry of various http services.
    /// </summary>
    public interface IHttpAlertingService
    {
        /// <summary>
        /// Inspects an http response to check if it met an alert condition or not.
        /// </summary>
        /// <param name="restResponse">The converted http response.</param>
        /// <param name="requestCondition">The alert condition set at the request-level.</param>
        /// <param name="clientCondition">The alert condition set at the client-level.</param>
        /// <returns>True if the response met an alert condition, false otherwise.</returns>
        bool IsHttpAlert(
            BaseRestResponse restResponse,
            AlertCondition<BaseRestResponse>? requestCondition,
            AlertCondition<BaseRestResponse>? clientCondition);

        /// <summary>
        /// Inspects the default and request <see cref="AlertCondition{T}"/>s to see if they required
        /// throwing exceptions upon an alert condition being met. If so, throw accordingly.
        /// </summary>
        /// <param name="requestCondition">The <see cref="AlertCondition{T}"/> from the request.</param>
        /// <param name="clientCondition">The <see cref="AlertCondition{T}"/> from the client.</param>
        /// <exception cref="AlertConditionMetException">
        /// Thrown when an <see cref="AlertCondition{T}"/> is met, and it requires throwing.
        /// All additionally assigned exceptions are thrown as inner exceptions to the base alert exception.
        /// </exception>
        void ThrowIfAlertedAndNeedingException(
            AlertCondition<BaseRestResponse>? requestCondition,
            AlertCondition<BaseRestResponse>? clientCondition);
    }
}