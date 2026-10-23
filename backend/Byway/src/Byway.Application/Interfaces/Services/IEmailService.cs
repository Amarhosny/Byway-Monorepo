namespace Byway.Application.Interfaces.Services;

/// <summary>
/// Bonus feature. Deliberately a single method — no template engine, no
/// queue. Application composes the subject/body strings itself; this just
/// sends them.
/// </summary>
public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
}
