using FluentValidation;
using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.API.Validators;

public sealed class GetContactsRequestValidator : AbstractValidator<GetContactsRequest>
{
    public GetContactsRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request.Search)
            .MaximumLength(1000)
            .When(request => !string.IsNullOrWhiteSpace(request.Search));

        RuleFor(request => request.Email)
            .MaximumLength(400)
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.PhoneNumber)
            .MaximumLength(50)
            .When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber));

        RuleFor(request => request.SortBy)
            .IsInEnum();

        RuleFor(request => request.SortDirection)
            .IsInEnum();

        RuleFor(request => request)
            .Must(request => !request.BirthdayFrom.HasValue || !request.BirthdayTo.HasValue || request.BirthdayFrom <= request.BirthdayTo)
            .WithMessage("BirthdayFrom must be earlier than or equal to BirthdayTo.");

        RuleFor(request => request)
            .Must(request => !request.CreatedFromUtc.HasValue || !request.CreatedToUtc.HasValue || request.CreatedFromUtc <= request.CreatedToUtc)
            .WithMessage("CreatedFromUtc must be earlier than or equal to CreatedToUtc.");
    }
}
