using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.Alerting;

namespace HumanaEdge.Webcore.Framework.Rest.Alerting
{
    /// <inheritdoc />
    public sealed class HttpAlertingService : IHttpAlertingService
    {
        /// <inheritdoc />
        public bool IsHttpAlert(
            BaseRestResponse? restResponse,
            AlertCondition? requestCondition,
            AlertCondition? clientCondition)
        {
            if (requestCondition != null)
            {
                return requestCondition.Condition.Invoke(restResponse);
            }

            if (clientCondition != null)
            {
                return clientCondition.Condition.Invoke(restResponse);
            }

            return false;
        }

        /// <inheritdoc />
        public void ThrowIfAlertedAndNeedingException(
            AlertCondition? requestCondition,
            AlertCondition? clientCondition)
        {
            MessageAppException exception = null!;
            if (requestCondition != null)
            {
                if (requestCondition.ThrowOnFailure is true)
                {
                    exception = new AlertConditionMetException(requestCondition.Exception);
                }
                else if (requestCondition.ThrowOnFailure is false)
                {
                    // explicitly not throwing per the rest request.
                    return;
                }
                else
                {
                    // if an alert condition is set, but the throw flag is unset, then obey client-level rules.
                }
            }

            if (clientCondition?.ThrowOnFailure is true)
            {
                exception = new AlertConditionMetException(clientCondition.Exception);
            }

            if (exception != null)
            {
                throw exception;
            }
        }
    }
}