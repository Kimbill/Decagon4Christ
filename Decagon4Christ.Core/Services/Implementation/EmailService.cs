using Decagon4Christ.Core.Services.Interface;
using Decagon4Christ.Data;
using Decagon4Christ.Model;
using Decagon4Christ.Model.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace Decagon4Christ.Core.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly string _fromMail;
        private readonly string _host;
        private readonly int _port;
        private readonly string _password;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(EmailSettings mailSettings, AppDbContext dbContext, IHttpContextAccessor contextAccessor, ILogger<EmailService> logger, IConfiguration config)
        {
            _fromMail = mailSettings.Username!;
            _host = mailSettings.Host!;
            _port = mailSettings.Port!;
            _password = mailSettings.Password!;
            _dbContext = dbContext;
            _httpContextAccessor = contextAccessor;
            _logger = logger;
            _config = config;
        }

        public async Task SendMail(UserAction userAction, string userEmail)
        {
            try
            {
                string templateContent = "";

                templateContent = await ReadEmailTemplateFromFile("MonthlyMessages.html");

                var request = new MailRequest
                {
                    ToMail = userEmail!,
                    Subject = "Decagon4Christ",
                    Purpose = ((int)userAction).ToString(),
                    Body = string.IsNullOrEmpty(templateContent) ? "" : templateContent
                };

                using var email = new MailMessage(_fromMail, userEmail);
                email.Subject = request.Subject;
                email.Body = request.Body;
                email.IsBodyHtml = true;

                var builder = new BodyBuilder();
                builder.HtmlBody = request.Body;

                using var smtp = new SmtpClient(_host, _port);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_fromMail, _password);

                await smtp.SendMailAsync(email);

                _logger.LogInformation($"Email sent to {request.ToMail} with purpose {request.Purpose}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email");
                throw;
            }
        }

        private async Task<string> ReadEmailTemplateFromFile(string templateFileName)
        {
            try
            {
                var templatePath = Path.Combine("StaticFiles", templateFileName);

                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"Email template file not found: {templateFileName}");
                    return string.Empty;
                }

                var templateContent = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);


                return templateContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading email template file: {templateFileName}");
                return string.Empty;
            }
        }

        private EmailPurpose GetPurposeFromUserAction(UserAction userAction)
        {
            switch (userAction)
            {
                case UserAction.Registration:
                    return EmailPurpose.Registration;

                case UserAction.PasswordReset:
                    return EmailPurpose.PasswordReset;

                case UserAction.Newsletter:
                    return EmailPurpose.Newsletter;

                default:
                    throw new InvalidOperationException("Unable to determine the email purpose.");
            }
        }

        private string GenerateEmailBody(string body, UserAction userAction)
        {
            string registrationLink = GenerateRegistrationLink(userAction);

            string emailBody = body.Replace("{registrationLink}", registrationLink);

            return emailBody;
        }

        private string GenerateRegistrationLink(UserAction userAction)
        {
            string userId = GetUserIdFromHttpContext();
            string token = GenerateUniqueToken();

            string registrationLink = $"https://example.com/register?userId={userId}&token={token}";

            return registrationLink;
        }

        private string GetUserIdFromHttpContext()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }

        private string GenerateUniqueToken()
        {
            const int tokenLength = 10;
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var random = new Random();
            var token = new StringBuilder();

            for (int i = 0; i < tokenLength; i++)
            {
                int randomIndex = random.Next(0, allowedChars.Length);
                char randomChar = allowedChars[randomIndex];
                token.Append(randomChar);
            }

            return token.ToString();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            try
            {
                using (var message = new MailMessage(_fromMail, toEmail))
                {
                    message.Subject = subject;
                    message.Body = content;
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(_host, _port))
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_fromMail, _password);

                        await client.SendMailAsync(message);
                    }
                }
                _logger.LogInformation($"Email sent to {toEmail} with subject {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email");
                throw;
            }
        }

    }
}