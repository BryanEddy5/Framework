using System;
using FluentValidation;

namespace HumanaEdge.Webcore.Core.DependencyInjection.Validators
{
    /// <summary>
    /// Attribute which is used to designed a given class as a validator for a fixed model.
    /// </summary>
    public sealed class ValidatorAttribute : DiComponent
    {
        /// <summary>
        /// Designated constructor.
        /// </summary>
        /// <param name="contractType">The type of contract that is validated by the attributed class.</param>
        public ValidatorAttribute(Type contractType)
        {
            Target = typeof(IValidator<>).MakeGenericType(contractType);
        }
    }
}