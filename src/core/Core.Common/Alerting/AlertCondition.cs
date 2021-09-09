using System;
using HumanaEdge.Webcore.Core.Common.Exceptions;

namespace HumanaEdge.Webcore.Core.Common.Alerting
{
    /// <summary>
    /// The structure of an alert condition.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class AlertCondition
    {
        /// <summary>
        /// A func to determine whether or not to toggle the alert flag on telemetry.
        /// </summary>
        /// <remarks>
        /// In the context of building a RESTful alert condition, the object? input can typically
        /// be converted to a RestResponse to further inspect it.<br/><br/>
        /// In the context of building a PubSub alert condition, the object? input is only
        /// a boolean representing the success of the publishing.
        /// </remarks>
        public Func<object?, bool> Condition { get; set; } = null!;

        /// <summary>
        /// Whether or not this alert will trigger a throw if it is met.
        /// </summary>
        public bool? ThrowOnFailure { get; set; }

        /// <summary>
        /// The exception to be thrown if an alert condition is met- defaults to
        /// <see cref="AlertConditionMetException"/> if one is not assigned.
        /// </summary>
        public MessageAppException? Exception { get; set; } = null;
    }
}