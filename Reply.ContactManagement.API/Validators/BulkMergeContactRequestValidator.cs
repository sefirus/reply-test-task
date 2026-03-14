using FluentValidation;
using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.API.Validators;

public sealed class BulkMergeContactRequestValidator : AbstractValidator<BulkMergeContactRequest>
{
    public BulkMergeContactRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(400)
            .EmailAddress();

        RuleFor(request => request.PhoneNumber)
            .MaximumLength(50)
            .When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber));

        RuleForEach(request => request.CustomFields)
            .SetValidator(new ContactCustomFieldValueRequestValidator());

        RuleFor(request => request.CustomFields)
            .Must(HaveUniqueKeys)
            .WithMessage("Custom field keys must be unique within a contact.");
    }

    private static bool HaveUniqueKeys(IReadOnlyCollection<ContactCustomFieldValueRequest>? customFields)
    {
        if (customFields is null)
        {
            return true;
        }

        return customFields
            .Select(customField => customField.Key.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() == customFields.Count;
    }
}
