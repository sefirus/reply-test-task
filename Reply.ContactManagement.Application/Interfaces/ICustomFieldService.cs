using Reply.ContactManagement.Core.Contracts.Common;
using Reply.ContactManagement.Core.Contracts.Contacts;
using Reply.ContactManagement.Core.Contracts.CustomFields;

namespace Reply.ContactManagement.Application.Interfaces;

public interface ICustomFieldService
{
    Task<IReadOnlyCollection<CustomFieldResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CreatedEntityResponse> CreateAsync(CreateCustomFieldRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ContactCustomFieldValueResponse> AssignValueAsync(
        Guid contactId,
        Guid customFieldId,
        AssignCustomFieldValueRequest request,
        CancellationToken cancellationToken = default);
}
