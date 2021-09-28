using FluentValidation;
using HumanaEdge.Webcore.Core.DependencyInjection.Validators;

namespace HumanaEdge.Webcore.Example.WebApi.PubSub.Validators
{
    /// <summary>
    /// Here's some Foo.
    /// </summary>
    [Validator(typeof(FooContract))]
    public class FooContractValidator : AbstractValidator<FooContract>
    {
        /// <summary>
        /// ctor.
        /// </summary>
        public FooContractValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}