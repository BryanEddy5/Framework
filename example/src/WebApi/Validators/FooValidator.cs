using FluentValidation;
using HumanaEdge.Webcore.Core.DependencyInjection.Validators;
using HumanaEdge.Webcore.Example.WebApi.Models;

namespace HumanaEdge.Webcore.Example.WebApi.Validators
{
    /// <summary>
    /// Here's some Foo.
    /// </summary>
    [Validator(typeof(Foo))]
    public class FooValidator : AbstractValidator<Foo>
    {
        /// <summary>
        /// ctor.
        /// </summary>
        public FooValidator()
        {
            RuleFor(x => x.Bar).NotEmpty();
        }
    }
}