using Byway.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Byway.Infrastructure.Services;

/// <summary>
/// Bonus feature. Sends via SMTP using whatever host/port/credentials are in
/// configuration — point this at Mailtrap for development (it "catches"
/// mail instead of really delivering it, which is exactly what you want
/// while testing registration/purchase emails). Swap SMTP settings for a
/// real provider later; nothing else needs to change since Application only
/// ever depends on IEmailService.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        var host = _config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            // No SMTP configured (e.g. local dev without a Mailtrap inbox set up) —
            // log instead of throwing, since email is a bonus feature and must
            // never block registration/checkout from completing.
            _logger.LogInformation("Email skipped (no SMTP configured). To: {To}, Subject: {Subject}", toEmail, subject);
            return;
        }

        using var client = new SmtpClient(host, int.Parse(_config["Smtp:Port"] ?? "587"))
        {
            Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
            EnableSsl = true
        };

        var message = new MailMessage(_config["Smtp:From"] ?? "no-reply@byway.local", toEmail, subject, body);

        try
        {
            await client.SendMailAsync(message, ct);
        }
        catch (Exception ex)
        {
            // Per requirement, email is bonus — a failed send must not fail
            // registration or checkout.
            _logger.LogWarning(ex, "Failed to send email to {To}", toEmail);
        }
    }
}
