using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RealEstateApp.Application.Dtos.Email;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Settings;
using System.Text.RegularExpressions;

namespace RealEstateApp.Infraestructure.Shared.Service
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings?.Value ?? throw new ArgumentNullException(nameof(mailSettings));
            _logger = logger;
        }

        public async Task SendAsync(EmailRequestDto emailRequestDto)
        {
            if (emailRequestDto == null) throw new ArgumentNullException(nameof(emailRequestDto));

            try
            {
                if (string.IsNullOrWhiteSpace(emailRequestDto.Subject) || string.IsNullOrWhiteSpace(emailRequestDto.HtmlBody))
                {
                    _logger.LogWarning("Email subject or html body is empty. To: {To}", emailRequestDto.To);
                    return;
                }

                var recipients = new List<string>();
                if (emailRequestDto.ToRange != null && emailRequestDto.ToRange.Any())
                    recipients.AddRange(emailRequestDto.ToRange.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));

                if (!string.IsNullOrWhiteSpace(emailRequestDto.To))
                    recipients.Add(emailRequestDto.To.Trim());

                if (!recipients.Any())
                {
                    _logger.LogWarning("No recipients specified for email with subject {Subject}", emailRequestDto.Subject);
                    return;
                }

                var message = new MimeMessage();

                var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.DisplayName)
                    ? new MailboxAddress(_mailSettings.DisplayName, _mailSettings.EmailFrom ?? _mailSettings.SmtpUser ?? string.Empty)
                    : MailboxAddress.Parse(_mailSettings.EmailFrom ?? _mailSettings.SmtpUser ?? throw new InvalidOperationException("EmailFrom or SmtpUser not configured"));

                message.From.Add(fromAddress);
                message.Sender = fromAddress;
                message.Subject = emailRequestDto.Subject ?? string.Empty;

                foreach (var to in recipients.Distinct())
                {
                    try
                    {
                        message.To.Add(MailboxAddress.Parse(to));
                    }
                    catch (Exception parseEx)
                    {
                        _logger.LogWarning(parseEx, "Invalid recipient skipped: {Recipient}", to);
                    }
                }

                var builder = new BodyBuilder
                {
                    HtmlBody = emailRequestDto.HtmlBody,
                };

                message.Body = builder.ToMessageBody();

                using var smtpClient = new SmtpClient();

                await smtpClient.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);

                if (!string.IsNullOrWhiteSpace(_mailSettings.SmtpUser))
                {
                    await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                }

                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation("Email enviado a {Recipients} (Subject: {Subject})", string.Join(',', recipients), message.Subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo. To: {To} Subject: {Subject}", emailRequestDto.To ?? string.Join(',', emailRequestDto.ToRange ?? new()), emailRequestDto.Subject);
                throw;
            }
        }

        private static string StripHtml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            try
            {
                var withoutTags = Regex.Replace(html, "<.*?>", string.Empty);
                return System.Net.WebUtility.HtmlDecode(withoutTags).Trim();
            }
            catch
            {
                return html ?? string.Empty;
            }
        }
    }
}