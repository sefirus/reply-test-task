using FluentValidation;
using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.API.Validators;

public sealed class BulkMergeContactsRequestValidator : AbstractValidator<BulkMergeContactsRequest>
{
    public BulkMergeContactsRequestValidator()
    {
        RuleFor(request => request.Contacts)
            .NotNull();

        RuleForEach(request => request.Contacts)
            .SetValidator(new BulkMergeContactRequestValidator());

        RuleFor(request => request.Contacts)
            .Must(HaveUniqueEmails)
            .WithMessage("Bulk merge payload must contain unique email addresses.");
    }

    private static bool HaveUniqueEmails(IReadOnlyCollection<BulkMergeContactRequest> contacts)
    {
        return contacts
            .Select(contact => contact.Email.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() == contacts.Count;
    }
}
