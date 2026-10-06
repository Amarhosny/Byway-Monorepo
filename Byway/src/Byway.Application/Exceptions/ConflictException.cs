namespace Byway.Application.Exceptions;

/// <summary>
/// Maps to HTTP 409. Used for business-rule violations that are about
/// STATE conflicts, not malformed input — e.g. "course already purchased",
/// "instructor still assigned to a course", "course already in cart".
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
