using System;

namespace HumanaEdge.Webcore.Core.Common.Validators
{
    /// <summary>
    /// Validates method arguments.
    /// </summary>
    public static class ArgumentValidator
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException" /> if the object is null or a string is empty.
        /// </summary>
        /// <param name="obj">The argument to be validated.</param>
        /// <typeparam name="T">The type to be inspected.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if the argument is not valid.</exception>
        /// <returns>Return the same object for fluent chaining.</returns>
        public static T AssertNotNullOrEmpty<T>(this T obj)
        {
            switch (obj)
            {
                case null:
                    throw new ArgumentNullException($"{nameof(obj)} cannot be null");

                case string str when string.IsNullOrEmpty(str):
                    throw new ArgumentNullException($"{nameof(obj)} cannot be null or be an empty string");
            }

            return obj;
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException" /> if the object is null or a string is empty.
        /// </summary>
        /// <param name="string">The argument to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown if the argument is not valid.</exception>
        /// <returns>Return the same string for fluent chaining.</returns>
        public static string AssertNotNullOrWhiteSpace(this string @string)
        {
            if (string.IsNullOrWhiteSpace(@string))
            {
                throw new ArgumentException($"{nameof(@string)} cannot be null or have white space.");
            }

            return @string;
        }
    }
}