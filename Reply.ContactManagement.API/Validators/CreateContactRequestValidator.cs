using FluentValidation;
using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.API.Validators;

public sealed class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
{
    public CreateContactRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(request => request.Email)
            .MaximumLength(400)
            .EmailAddress()
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.PhoneNumber)
            .MaximumLength(50)
            .When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber));
    }
}
