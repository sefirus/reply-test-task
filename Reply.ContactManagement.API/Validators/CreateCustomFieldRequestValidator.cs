using FluentValidation;
using Reply.ContactManagement.Core.Contracts.CustomFields;

namespace Reply.ContactManagement.API.Validators;

public sealed class CreateCustomFieldRequestValidator : AbstractValidator<CreateCustomFieldRequest>
{
    public CreateCustomFieldRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(request => request.DataType)
            .IsInEnum();
    }
}
