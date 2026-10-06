namespace Byway.Application.Exceptions;

/// <summary>Maps to HTTP 404 in the API's global exception-handling middleware.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.") { }
}
