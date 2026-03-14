using System.Text.Json;
using FluentValidation;
using Reply.ContactManagement.Core.Contracts.CustomFields;

namespace Reply.ContactManagement.API.Validators;

public sealed class AssignCustomFieldValueRequestValidator : AbstractValidator<AssignCustomFieldValueRequest>
{
    public AssignCustomFieldValueRequestValidator()
    {
        RuleFor(request => request.Value.ValueKind)
            .NotEqual(JsonValueKind.Undefined);

        RuleFor(request => request.Value)
            .Must(value => value.ValueKind != JsonValueKind.String || value.GetString() is null || value.GetString()!.Length <= 16000)
            .WithMessage("Custom field string values must be 16000 characters or fewer.");
    }
}
