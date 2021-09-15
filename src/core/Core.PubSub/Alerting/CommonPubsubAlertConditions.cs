using HumanaEdge.Webcore.Core.Common.Alerting;

namespace HumanaEdge.Webcore.Core.PubSub.Alerting
{
    /// <summary>
    /// A couple pre-made <see cref="AlertCondition{T}"/>s for use with the <see cref="IPublisherClient{T}"/>.
    /// </summary>
    public static class CommonPubsubAlertConditions
    {
        /// <summary>
        /// A non-conditional <see cref="AlertCondition{T}"/>.
        /// </summary>
        /// <returns>A non-conditional <see cref="AlertCondition{T}"/> .</returns>
        /// <remarks>No alert will ever be flagged in the telemetry where this is used.</remarks>
        public static AlertCondition<bool> None()
        {
            return new AlertCondition<bool>
            {
                Condition = _ => false,
                ThrowOnFailure = false
            };
        }

        /// <summary>
        /// A standard alert condition, the default.<br/>
        /// If the success was false from sending, then an alert will be flagged in the telemetry.
        /// </summary>
        /// <returns>The standard <see cref="AlertCondition{T}"/>.</returns>
        /// <remarks>No alert will ever be flagged in the telemetry where this is used.</remarks>
        public static AlertCondition<bool> Standard()
        {
            bool StandardAlerting(bool success) => success == false;
            return new AlertCondition<bool>
            {
                Condition = StandardAlerting,
                ThrowOnFailure = false
            };
        }
    }
}