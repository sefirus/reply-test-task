using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.Application.Interfaces;

public interface IContactBulkMergeService
{
    Task<BulkMergeContactsResponse> MergeAsync(
        BulkMergeContactsRequest request,
        CancellationToken cancellationToken = default);
}
