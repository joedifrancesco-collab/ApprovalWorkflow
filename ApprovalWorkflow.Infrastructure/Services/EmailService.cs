using System.Net;
using System.Net.Mail;
using ApprovalWorkflow.Application.Interfaces;

namespace ApprovalWorkflow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(SmtpSettings settings)
    {
        _settings = settings;
    }

    public async Task SendPasswordResetAsync(string toEmail, string toName, string resetUrl)
    {
        // Development fallback: if SMTP Host is not configured, print the link to the console
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            Console.WriteLine();
            Console.WriteLine("=== PASSWORD RESET (SMTP not configured) ===");
            Console.WriteLine($"  To:  {toEmail}");
            Console.WriteLine($"  URL: {resetUrl}");
            Console.WriteLine("============================================");
            Console.WriteLine();
            return;
        }

        var safeName = WebUtility.HtmlEncode(toName);
        var safeUrl  = WebUtility.HtmlEncode(resetUrl);

        var body = $"""
            <div style="font-family:'Segoe UI',Arial,sans-serif;max-width:520px;margin:0 auto;padding:32px;">
              <h2 style="color:#1a1a2e;margin-bottom:8px;">Password Reset Request</h2>
              <p style="color:#555;">Hi {safeName},</p>
              <p style="color:#555;">
                We received a request to reset your Approval Workflow password.
                Click the button below to set a new password. This link expires in&nbsp;<strong>1&nbsp;hour</strong>.
              </p>
              <p style="margin:28px 0;">
                <a href="{safeUrl}"
                   style="display:inline-block;background:#4f46e5;color:#fff;padding:12px 28px;
                          border-radius:8px;text-decoration:none;font-weight:600;font-size:1rem;">
                  Reset Password
                </a>
              </p>
              <p style="color:#888;font-size:0.85em;">
                If you didn't request this, you can safely ignore this email.
                Your password will not change until you click the link above.
              </p>
              <hr style="border:none;border-top:1px solid #eee;margin:24px 0;" />
              <p style="color:#aaa;font-size:0.78em;">Approval Workflow &mdash; automated message, please do not reply.</p>
            </div>
            """;

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
            EnableSsl   = _settings.EnableSsl
        };

        var msg = new MailMessage
        {
            From       = new MailAddress(_settings.FromAddress, _settings.FromDisplayName),
            Subject    = "Reset your Approval Workflow password",
            IsBodyHtml = true,
            Body       = body
        };
        msg.To.Add(new MailAddress(toEmail, toName));

        await client.SendMailAsync(msg);
    }
}
