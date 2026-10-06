namespace Byway.Application.Exceptions;

/// <summary>Maps to HTTP 403 — e.g. reviewing a course the user never purchased.</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
