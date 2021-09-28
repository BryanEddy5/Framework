using System.Collections.Generic;
using FluentValidation.Results;
using HumanaEdge.Webcore.Core.PubSub;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions
{
    /// <summary>
    /// An exception thrown when the model state is invalid.
    /// </summary>
    internal sealed class InvalidModelStateException : PubSubException
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="errors">The bad exception response.</param>
        public InvalidModelStateException(IList<ValidationFailure> errors)
            : base(
                $"The model state is invalid with the following errors {string.Join(";", errors)}.",
                "The model state was invalid with these {@Errors}",
                errors)
        {
        }

        /// <inheritdoc />
        public override Reply Reply => Reply.Ack;
    }
}