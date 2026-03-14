using System.Text.Json;
using FluentValidation;
using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.API.Validators;

public sealed class ContactCustomFieldValueRequestValidator : AbstractValidator<ContactCustomFieldValueRequest>
{
    public ContactCustomFieldValueRequestValidator()
    {
        RuleFor(request => request.Key)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(request => request.Value.ValueKind)
            .NotEqual(JsonValueKind.Undefined);

        RuleFor(request => request.Value)
            .Must(value => value.ValueKind != JsonValueKind.String || value.GetString() is null || value.GetString()!.Length <= 16000)
            .WithMessage("Custom field string values must be 16000 characters or fewer.");
    }
}
