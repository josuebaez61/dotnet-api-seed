using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CleanArchitecture.Application.Common.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger,
            ILocalizationService localizationService,
            IEmailTemplateService emailTemplateService)
        {
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpHost = emailSettings["SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var smtpUsername = emailSettings["SmtpUsername"] ?? "";
                var smtpPassword = emailSettings["SmtpPassword"] ?? "";
                var fromEmail = emailSettings["FromEmail"] ?? "noreply@cleanarchitecture.com";
                var fromName = emailSettings["FromName"] ?? "Clean Architecture";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                throw new EmailSendingFailedError();
            }
        }

        public async Task SendPasswordResetEmailAsync(string to, string userName, string resetCode)
        {
            var culture = _localizationService.GetCurrentCulture();

            var parameters = new Dictionary<string, object>
            {
                ["UserName"] = userName,
                ["ResetCode"] = resetCode
            };

            var body = await _emailTemplateService.RenderEmailAsync("PasswordReset", culture, parameters);
            var subject = _localizationService.GetSubjectMessage("PASSWORD_RESET");

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string to, string userName)
        {
            var culture = _localizationService.GetCurrentCulture();
            var parameters = new Dictionary<string, object>
            {
                ["UserName"] = userName
            };

            var body = await _emailTemplateService.RenderEmailAsync("Welcome", culture, parameters);
            var subject = _localizationService.GetSubjectMessage("WELCOME");

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordChangedEmailAsync(string to, string userName)
        {
            var culture = _localizationService.GetCurrentCulture();
            var parameters = new Dictionary<string, object>
            {
                ["UserName"] = userName
            };

            var body = await _emailTemplateService.RenderEmailAsync("PasswordChanged", culture, parameters);
            var subject = _localizationService.GetSubjectMessage("PASSWORD_CHANGED");

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendEmailChangeVerificationEmailAsync(string to, string userName, string verificationCode)
        {
            var culture = _localizationService.GetCurrentCulture();

            var parameters = new Dictionary<string, object>
            {
                ["UserName"] = userName,
                ["VerificationCode"] = verificationCode
            };

            var body = await _emailTemplateService.RenderEmailAsync("EmailChangeVerification", culture, parameters);
            var subject = _localizationService.GetSubjectMessage("EMAIL_CHANGE_VERIFICATION");

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendEmailChangeConfirmationEmailAsync(string to, string userName, string oldEmail)
        {
            var culture = _localizationService.GetCurrentCulture();
            var parameters = new Dictionary<string, object>
            {
                ["UserName"] = userName,
                ["OldEmail"] = oldEmail
            };

            var body = await _emailTemplateService.RenderEmailAsync("EmailChangeConfirmation", culture, parameters);
            var subject = _localizationService.GetSubjectMessage("EMAIL_CHANGE_CONFIRMATION");

            await SendEmailAsync(to, subject, body);
        }
        public async Task SendTemporaryPasswordEmailAsync(string to, string userName, string temporaryPassword)
        {
            var culture = _localizationService.GetCurrentCulture();
            var parameters = new Dictionary<string, object>
            {
                ["UserName"] = userName,
                ["TemporaryPassword"] = temporaryPassword
            };

            var body = await _emailTemplateService.RenderEmailAsync("TemporaryPassword", culture, parameters);
            var subject = _localizationService.GetSubjectMessage("TEMPORARY_PASSWORD");

            await SendEmailAsync(to, subject, body);
        }
    }
}
