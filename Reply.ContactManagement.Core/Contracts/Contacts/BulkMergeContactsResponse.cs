namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record BulkMergeContactsResponse(
    int CreatedCount,
    int UpdatedCount,
    int DeletedCount);
