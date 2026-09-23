using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using VitalityPortal.Models.Payments;

namespace VitalityPortal.Services;

public interface IEmailSenderService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
}

public sealed class EmailSenderService(IOptions<GmailSmtpOptions> options, ILogger<EmailSenderService> logger) : IEmailSenderService
{
    private readonly GmailSmtpOptions _config = options.Value;

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(toEmail)) return false;

        // If no AppPassword configured yet, log delivery gracefully
        if (string.IsNullOrWhiteSpace(_config.AppPassword))
        {
            logger.LogInformation("[Email Simulation] To: {To}, Subject: {Subject}, Body: {Body}", toEmail, subject, htmlBody);
            return true;
        }

        try
        {
            using var client = new SmtpClient(_config.Host, _config.Port)
            {
                Credentials = new NetworkCredential(_config.SenderEmail, _config.AppPassword),
                EnableSsl = _config.EnableSsl
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_config.SenderEmail, _config.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            logger.LogInformation("Email sent successfully to {To}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", toEmail);
            return false;
        }
    }
}
