namespace Reply.ContactManagement.Core.Contracts.Contacts;

public sealed record BulkMergeContactsRequest(
    IReadOnlyCollection<BulkMergeContactRequest> Contacts);
