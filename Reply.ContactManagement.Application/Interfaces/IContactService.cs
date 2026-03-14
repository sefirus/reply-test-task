using Reply.ContactManagement.Core.Contracts.Common;
using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.Application.Interfaces;

public interface IContactService
{
    Task<CreatedEntityResponse> CreateAsync(CreateContactRequest request, CancellationToken cancellationToken = default);

    Task<PagedResponse<ContactResponse>> GetAsync(GetContactsRequest request, CancellationToken cancellationToken = default);

    Task<ContactResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ContactResponse> UpdateAsync(Guid id, UpdateContactRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
