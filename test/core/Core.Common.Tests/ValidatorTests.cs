using System.ComponentModel.DataAnnotations;
using HumanaEdge.Webcore.Core.Common.Validators;
using Xunit;

namespace HumanaEdge.Webcore.Core.Common.Tests
{
    /// <summary>
    ///     Example test validation tests for models.
    /// </summary>
    public class ValidatorTests
    {
        /// <summary>
        ///     Validates that the model has a required property.
        /// </summary>
        [Fact]
        public void TestMissingEnumValue_ReturnsValidationException()
        {
            // arrange
            var entity = new Entity();

            // assert
            Assert.Throws<ValidationException>(
                () =>
                    Validator.ValidateObject(entity, new ValidationContext(entity, null, null), true));
        }

        /// <summary>
        ///     Some model that certainly is not Foo.
        /// </summary>
        internal class Entity
        {
            /// <summary>
            ///     Require enum value Foo.
            /// </summary>
            [RequiredEnum]
            public TestEnum EnumValue { get; set; }
        }
    }
}