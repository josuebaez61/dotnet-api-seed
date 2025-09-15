using System;
using System.Threading.Tasks;
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

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger,
            ILocalizationService localizationService)
        {
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;
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
                throw new InvalidOperationException(_localizationService.GetErrorMessage("EMAIL_SENDING_FAILED"));
            }
        }

        public async Task SendPasswordResetEmailAsync(string to, string userName, string resetCode)
        {
            var subject = _localizationService.GetString("PASSWORD_RESET_SUBJECT");
            var body = GetPasswordResetEmailTemplate(userName, resetCode);
            await SendEmailAsync(to, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string to, string userName)
        {
            var subject = _localizationService.GetString("WELCOME_SUBJECT");
            var body = GetWelcomeEmailTemplate(userName);
            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordChangedEmailAsync(string to, string userName)
        {
            var subject = _localizationService.GetString("PASSWORD_CHANGED_SUBJECT");
            var body = GetPasswordChangedEmailTemplate(userName);
            await SendEmailAsync(to, subject, body);
        }

        private string GetPasswordResetEmailTemplate(string userName, string resetCode)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Restablecer Contraseña</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .container {{
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 24px;
            font-weight: bold;
            color: #2c3e50;
            margin-bottom: 10px;
        }}
        .code {{
            background-color: #3498db;
            color: white;
            padding: 15px 30px;
            font-size: 24px;
            font-weight: bold;
            text-align: center;
            border-radius: 5px;
            margin: 20px 0;
            letter-spacing: 3px;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #666;
            text-align: center;
        }}
        .warning {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>Clean Architecture</div>
            <h2>Restablecer Contraseña</h2>
        </div>
        
        <p>Hola <strong>{userName}</strong>,</p>
        
        <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta. Utiliza el siguiente código para continuar:</p>
        
        <div class='code'>{resetCode}</div>
        
        <div class='warning'>
            <strong>Importante:</strong> Este código expirará en 15 minutos por motivos de seguridad.
        </div>
        
        <p>Si no solicitaste este cambio, puedes ignorar este correo electrónico.</p>
        
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2024 Clean Architecture. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetWelcomeEmailTemplate(string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Bienvenido</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .container {{
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 24px;
            font-weight: bold;
            color: #2c3e50;
            margin-bottom: 10px;
        }}
        .welcome {{
            background-color: #27ae60;
            color: white;
            padding: 20px;
            border-radius: 5px;
            text-align: center;
            margin: 20px 0;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #666;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>Clean Architecture</div>
        </div>
        
        <div class='welcome'>
            <h2>¡Bienvenido a Clean Architecture!</h2>
        </div>
        
        <p>Hola <strong>{userName}</strong>,</p>
        
        <p>¡Gracias por registrarte en nuestra plataforma! Tu cuenta ha sido creada exitosamente.</p>
        
        <p>Ahora puedes:</p>
        <ul>
            <li>Iniciar sesión con tus credenciales</li>
            <li>Explorar todas las funcionalidades disponibles</li>
            <li>Personalizar tu perfil</li>
        </ul>
        
        <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
        
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2024 Clean Architecture. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetPasswordChangedEmailTemplate(string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Contraseña Cambiada</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .container {{
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 24px;
            font-weight: bold;
            color: #2c3e50;
            margin-bottom: 10px;
        }}
        .success {{
            background-color: #27ae60;
            color: white;
            padding: 20px;
            border-radius: 5px;
            text-align: center;
            margin: 20px 0;
        }}
        .warning {{
            background-color: #e74c3c;
            color: white;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #666;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>Clean Architecture</div>
        </div>
        
        <div class='success'>
            <h2>Contraseña Actualizada</h2>
        </div>
        
        <p>Hola <strong>{userName}</strong>,</p>
        
        <p>Tu contraseña ha sido cambiada exitosamente.</p>
        
        <div class='warning'>
            <strong>Importante:</strong> Si no realizaste este cambio, contacta inmediatamente con nuestro equipo de soporte.
        </div>
        
        <p>Para tu seguridad, te recomendamos:</p>
        <ul>
            <li>No compartir tu contraseña con nadie</li>
            <li>Usar una contraseña única para esta cuenta</li>
            <li>Cambiar tu contraseña regularmente</li>
        </ul>
        
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2024 Clean Architecture. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        public async Task SendEmailChangeVerificationEmailAsync(string to, string userName, string verificationCode)
        {
            var subject = _localizationService.GetString("EMAIL_CHANGE_VERIFICATION_SUBJECT");
            var body = GetEmailChangeVerificationEmailTemplate(userName, verificationCode);
            await SendEmailAsync(to, subject, body);
        }

        public async Task SendEmailChangeConfirmationEmailAsync(string to, string userName, string oldEmail)
        {
            var subject = _localizationService.GetString("EMAIL_CHANGE_CONFIRMATION_SUBJECT");
            var body = GetEmailChangeConfirmationEmailTemplate(userName, oldEmail);
            await SendEmailAsync(to, subject, body);
        }

        private string GetEmailChangeVerificationEmailTemplate(string userName, string verificationCode)
        {
            var frontendUrl = "http://localhost:4200"; // TODO: Make this configurable
            var verificationUrl = $"{frontendUrl}/auth/confirm-email?code={verificationCode}";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Verificar Cambio de Email</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .container {{
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 24px;
            font-weight: bold;
            color: #2c3e50;
            margin-bottom: 10px;
        }}
        .button {{
            display: inline-block;
            background-color: #3498db;
            color: white;
            padding: 15px 30px;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
            font-weight: bold;
        }}
        .button:hover {{
            background-color: #2980b9;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #666;
            text-align: center;
        }}
        .warning {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .code {{
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            padding: 10px;
            border-radius: 5px;
            font-family: monospace;
            word-break: break-all;
            margin: 10px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>Clean Architecture</div>
            <h2>Verificar Cambio de Email</h2>
        </div>
        
        <p>Hola <strong>{userName}</strong>,</p>
        
        <p>Hemos recibido una solicitud para cambiar tu dirección de correo electrónico. Para completar este cambio, haz clic en el siguiente enlace:</p>
        
        <div style='text-align: center;'>
            <a href='{verificationUrl}' class='button'>Verificar Cambio de Email</a>
        </div>
        
        <p>Si el botón no funciona, copia y pega la siguiente URL en tu navegador:</p>
        <div class='code'>{verificationUrl}</div>
        
        <p>O puedes usar este código de verificación directamente en la aplicación:</p>
        <div class='code'>{verificationCode}</div>
        
        <div class='warning'>
            <strong>Importante:</strong> Este enlace expirará en 24 horas por motivos de seguridad.
        </div>
        
        <p>Si no solicitaste este cambio, puedes ignorar este correo electrónico.</p>
        
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2024 Clean Architecture. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetEmailChangeConfirmationEmailTemplate(string userName, string oldEmail)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Cambiado Exitosamente</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .container {{
            background-color: #ffffff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 24px;
            font-weight: bold;
            color: #2c3e50;
            margin-bottom: 10px;
        }}
        .success {{
            background-color: #d4edda;
            border: 1px solid #c3e6cb;
            color: #155724;
            padding: 20px;
            border-radius: 5px;
            text-align: center;
            margin: 20px 0;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #666;
            text-align: center;
        }}
        .warning {{
            background-color: #e74c3c;
            color: white;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>Clean Architecture</div>
        </div>
        
        <div class='success'>
            <h2>✅ Email Actualizado Exitosamente</h2>
        </div>
        
        <p>Hola <strong>{userName}</strong>,</p>
        
        <p>Tu dirección de correo electrónico ha sido cambiada exitosamente.</p>
        
        <p><strong>Email anterior:</strong> {oldEmail}</p>
        <p><strong>Email actual:</strong> {userName}</p>
        
        <div class='warning'>
            <strong>Importante:</strong> Si no realizaste este cambio, contacta inmediatamente con nuestro equipo de soporte.
        </div>
        
        <p>Ahora puedes usar tu nueva dirección de correo electrónico para:</p>
        <ul>
            <li>Iniciar sesión en tu cuenta</li>
            <li>Recibir notificaciones importantes</li>
            <li>Recuperar tu contraseña si es necesario</li>
        </ul>
        
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2024 Clean Architecture. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
