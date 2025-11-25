using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;
        public EmailTemplateService(ILogger<EmailTemplateService> logger, ILocalizationService localizationService, IConfiguration configuration)
        {
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
        }

        public async Task<string> RenderTemplateAsync(string templateName, string culture, Dictionary<string, object> parameters)
        {
            try
            {
                var templatePath = GetTemplatePath(templateName, culture);
                var templateContent = await LoadTemplateAsync(templatePath);

                return ReplaceParameters(templateContent, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering template {TemplateName} for culture {Culture}", templateName, culture);
                throw;
            }
        }

        public async Task<string> RenderEmailAsync(string templateName, string culture, Dictionary<string, object> parameters)
        {
            try
            {
                // Load the specific template content
                var templateContent = await RenderTemplateAsync(templateName, culture, parameters);

                // Load the base email template
                var baseTemplatePath = GetBaseTemplatePath();
                var baseTemplate = await LoadTemplateAsync(baseTemplatePath);

                // Prepare parameters for the base template
                var baseParameters = new Dictionary<string, object>(parameters)
                {
                    ["Content"] = templateContent,
                    ["Culture"] = culture,
                    ["Title"] = GetEmailTitle(templateName, culture),
                    ["HeaderTitle"] = GetHeaderTitle(templateName, culture),
                    ["CompanyName"] = _configuration.GetSection("CompanySettings:Name")?.Value ?? "Clean Architecture",
                    ["FooterMessage"] = GetFooterMessage(culture),
                    ["SupportEmail"] = _configuration.GetSection("CompanySettings:SupportEmail")?.Value ?? "support@cleanarchitecture.com"
                };

                return ReplaceParameters(baseTemplate, baseParameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering email {TemplateName} for culture {Culture}", templateName, culture);
                throw;
            }
        }

        private string GetTemplatePath(string templateName, string culture)
        {
            var fileName = templateName switch
            {
                "PasswordReset" => "password-reset.html",
                "Welcome" => "welcome.html",
                "PasswordChanged" => "password-changed.html",
                "EmailChangeVerification" => "email-change-verification.html",
                "EmailChangeConfirmation" => "email-change-confirmation.html",
                "TemporaryPassword" => "temporary-password.html",
                _ => throw new ArgumentException($"Unknown template name: {templateName}")
            };

            return $"CleanArchitecture.Application.Common.Templates.Email.{templateName}.{culture}.{fileName}";
        }

        private string GetBaseTemplatePath()
        {
            return "CleanArchitecture.Application.Common.Templates.Email.Base.email-template.html";
        }

        private async Task<string> LoadTemplateAsync(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Template resource not found: {resourceName}");
            }

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        private string ReplaceParameters(string template, Dictionary<string, object> parameters)
        {
            var result = template;

            foreach (var parameter in parameters)
            {
                var placeholder = $"{{{{{parameter.Key}}}}}";
                var value = parameter.Value?.ToString() ?? string.Empty;
                result = result.Replace(placeholder, value);
            }

            return result;
        }

        private string GetEmailTitle(string templateName, string culture)
        {
            return templateName switch
            {
                "PasswordReset" => _localizationService.GetSubjectMessage("PASSWORD_RESET"),
                "Welcome" => _localizationService.GetSubjectMessage("WELCOME"),
                "PasswordChanged" => _localizationService.GetSubjectMessage("PASSWORD_CHANGED"),
                "EmailChangeVerification" => _localizationService.GetSubjectMessage("EMAIL_CHANGE_VERIFICATION"),
                "EmailChangeConfirmation" => _localizationService.GetSubjectMessage("EMAIL_CHANGE_CONFIRMATION"),
                "TemporaryPassword" => _localizationService.GetSubjectMessage("TEMPORARY_PASSWORD"),
                _ => "Clean Architecture"
            };
        }

        private string GetHeaderTitle(string templateName, string culture)
        {
            return templateName switch
            {
                "PasswordReset" => culture == "es" ? "Restablecimiento de Contraseña" : "Password Reset",
                "Welcome" => culture == "es" ? "Bienvenido" : "Welcome",
                "PasswordChanged" => culture == "es" ? "Contraseña Cambiada" : "Password Changed",
                "EmailChangeVerification" => culture == "es" ? "Verificación de Cambio de Correo" : "Email Change Verification",
                "EmailChangeConfirmation" => culture == "es" ? "Correo Cambiado" : "Email Changed",
                "TemporaryPassword" => culture == "es" ? "Contraseña Temporal" : "Temporary Password",
                _ => "Clean Architecture"
            };
        }

        private string GetFooterMessage(string culture)
        {
            return culture == "es"
                ? "Este es un correo automático, por favor no respondas a este mensaje."
                : "This is an automated email, please do not reply to this message.";
        }
    }
}
