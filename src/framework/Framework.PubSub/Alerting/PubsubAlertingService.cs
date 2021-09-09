using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.PubSub.Alerting;

namespace HumanaEdge.Webcore.Framework.PubSub.Alerting
{
    /// <inheritdoc />
    public sealed class PubsubAlertingService : IPubsubAlertingService
    {
        /// <inheritdoc />
        public bool IsPubsubAlert(AlertCondition clientCondition, bool success)
        {
            return clientCondition.Condition.Invoke(success);
        }

        /// <param name="clientCondition"></param>
        /// <inheritdoc />
        public void ThrowIfAlertedAndNeedingException(AlertCondition clientCondition)
        {
            MessageAppException exception = null!;
            if (clientCondition.ThrowOnFailure is true)
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