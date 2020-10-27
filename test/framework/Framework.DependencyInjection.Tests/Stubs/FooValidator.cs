using FluentValidation;
using HumanaEdge.Webcore.Core.DependencyInjection.Validators;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Tests.Stubs
{
    /// <summary>
    /// Tests the DI registration using the <see cref="ValidatorAttribute"/>.
    /// </summary>
    [Validator(typeof(Foo))]
    public class FooValidator : AbstractValidator<Foo>
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        public FooValidator()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }
}