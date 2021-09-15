using HumanaEdge.Webcore.Core.Common.Alerting;

namespace HumanaEdge.Webcore.Core.Rest.Alerting
{
    /// <summary>
    /// A few pre-made <see cref="AlertCondition{T}"/>s for use with the <see cref="IRestClient"/>.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public static class CommonRestAlertConditions
    {
        /// <summary>
        /// A non-conditional <see cref="AlertCondition{T}"/>.
        /// </summary>
        /// <returns>A non-conditional <see cref="AlertCondition{T}"/> .</returns>
        /// <remarks>
        /// Can be overriden by a request-level <see cref="AlertCondition{T}"/> if this is used by the client.
        /// </remarks>
        public static AlertCondition<BaseRestResponse> None()
        {
            return new AlertCondition<BaseRestResponse>
            {
                Condition = _ => false,
                ThrowOnFailure = false
            };
        }

        /// <summary>
        /// A minimum alert condition, the default.<br/>
        /// If the response was null after conversion, then an alert will be flagged in the telemetry.
        /// </summary>
        /// <returns>The minimum <see cref="AlertCondition{T}"/>.</returns>
        /// <remarks>
        /// Can be overriden by a request-level <see cref="AlertCondition{T}"/> if this is used by the client.
        /// </remarks>
        public static AlertCondition<BaseRestResponse> Minimum()
        {
            bool MinimalAlerting(object? response) => response == null;
            return new AlertCondition<BaseRestResponse>
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
        /// <returns>A moderate <see cref="AlertCondition{BaseRestResponse}"/>.</returns>
        /// <remarks>
        /// Can be overriden by a request-level <see cref="AlertCondition{BaseRestResponse}"/> if this is used by the client.
        /// </remarks>
        public static AlertCondition<BaseRestResponse> Standard()
        {
            bool StandardAlerting(BaseRestResponse? response)
            {
                if (response == null)
                {
                    return true;
                }

                if ((int)response.StatusCode > 400 && (int)response.StatusCode != 404)
                {
                    return true;
                }

                return false;
            }

            return new AlertCondition<BaseRestResponse>
            {
                Condition = StandardAlerting,
                ThrowOnFailure = false
            };
        }
    }
}