namespace Reply.ContactManagement.Core.Exceptions;

public class EntityNotFoundException(string entityName, object key)
    : Exception($"{entityName} '{key}' was not found.");
