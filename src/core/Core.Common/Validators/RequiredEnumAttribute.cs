using System;
using System.ComponentModel.DataAnnotations;

namespace HumanaEdge.Webcore.Core.Common.Validators
{
    /// <summary>
    /// Validates that an Enum is not the default (e.g. zero) value.
    /// </summary>
    public class RequiredEnumAttribute : RequiredAttribute
    {
        /// <summary>
        /// Determines if the object is a valid <see cref="Enum" /> type that is not the default value.
        /// </summary>
        /// <param name="value">The object to test if it is a valid <see cref="Enum" />.</param>
        /// <returns>An identifier if it is valid.</returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            var type = value.GetType();

            return type.IsEnum &&
                   Enum.IsDefined(type, value) &&
                   (int)value != 0;
        }
    }
}