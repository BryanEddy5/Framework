using HumanaEdge.Webcore.Core.Common.Alerting;

namespace HumanaEdge.Webcore.Core.Rest.Alerting
{
    /// <summary>
    /// A few pre-made <see cref="AlertCondition"/>s for use with the <see cref="IRestClient"/>.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public static class CommonRestAlertConditions
    {
        /// <summary>
        /// A non-conditional <see cref="AlertCondition"/>.
        /// </summary>
        /// <returns>A non-conditional <see cref="AlertCondition"/> .</returns>
        /// <remarks>
        /// Can be overriden by a request-level <see cref="AlertCondition"/> if this is used by the client.
        /// </remarks>
        public static AlertCondition None()
        {
            return new AlertCondition
            {
                Condition = _ => false,
                ThrowOnFailure = false
            };
        }

        /// <summary>
        /// A minimum alert condition, the default.<br/>
        /// If the response was null after conversion, then an alert will be flagged in the telemetry.
        /// </summary>
        /// <returns>The minimum <see cref="AlertCondition"/>.</returns>
        /// <remarks>
        /// Can be overriden by a request-level <see cref="AlertCondition"/> if this is used by the client.
        /// </remarks>
        public static AlertCondition Minimum()
        {
            bool MinimalAlerting(object? response) => response == null;
            return new AlertCondition
            {
                Condition = MinimalAlerting,
                ThrowOnFailure = false
            };
        }

        /// <summary>
        /// A standard alert condition.<br/>
        /// Validates that the response is not null, and that the http status code was successful.<br/>
        /// The 404 status code is considered successful for the purposes of this alerting.
        /// </summary>
        /// <returns>A moderate <see cref="AlertCondition"/>.</returns>
        /// <remarks>
        /// Can be overriden by a request-level <see cref="AlertCondition"/> if this is used by the client.
        /// </remarks>
        public static AlertCondition Standard()
        {
            bool StandardAlerting(object? response)
            {
                if (response == null)
                {
                    return true;
                }

                var restResponse = (response as BaseRestResponse)!;
                if ((int)restResponse.StatusCode > 400 && (int)restResponse.StatusCode != 404)
                {
                    return true;
                }

                return false;
            }

            return new AlertCondition
            {
                Condition = StandardAlerting,
                ThrowOnFailure = false
            };
        }
    }
}