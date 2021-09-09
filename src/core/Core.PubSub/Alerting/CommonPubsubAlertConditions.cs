using HumanaEdge.Webcore.Core.Common.Alerting;

namespace HumanaEdge.Webcore.Core.PubSub.Alerting
{
    /// <summary>
    /// A couple pre-made <see cref="AlertCondition"/>s for use with the <see cref="IPublisherClient{T}"/>.
    /// </summary>
    public static class CommonPubsubAlertConditions
    {
        /// <summary>
        /// A non-conditional <see cref="AlertCondition"/>.
        /// </summary>
        /// <returns>A non-conditional <see cref="AlertCondition"/> .</returns>
        /// <remarks>No alert will ever be flagged in the telemetry where this is used.</remarks>
        public static AlertCondition None()
        {
            return new AlertCondition
            {
                Condition = _ => false,
                ThrowOnFailure = false
            };
        }

        /// <summary>
        /// A standard alert condition, the default.<br/>
        /// If the success was false from sending, then an alert will be flagged in the telemetry.
        /// </summary>
        /// <returns>The standard <see cref="AlertCondition"/>.</returns>
        /// <remarks>No alert will ever be flagged in the telemetry where this is used.</remarks>
        public static AlertCondition Standard()
        {
            bool StandardAlerting(object? success) => (bool)success! == false;
            return new AlertCondition
            {
                Condition = StandardAlerting,
                ThrowOnFailure = false
            };
        }
    }
}