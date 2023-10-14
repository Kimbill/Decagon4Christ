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
                var purpose = GetPurposeFromUserAction(userAction);
                var template = await GetEmailTemplateByPurpose(purpose.ToString());
                string registrationLink = GenerateRegistrationLink(userAction);

                var request = new MailRequest
                {
                    ToMail = userEmail!,
                    Subject = template.Subject,
                    Purpose = template.Purpose.ToString(),
                };
                if (userAction == UserAction.Registration || userAction == UserAction.PasswordReset)
                {
                    string emailBody = template.Body.Replace(template.Body, registrationLink);

                    request.Body = GenerateEmailBody(emailBody, userAction);
                }
                else
                {
                    request.Body = template.Body;
                }

                using var email = new MailMessage(_fromMail, userEmail);
                //email.Sender = MailboxAddress.Parse(_fromMail);
                //email.To.Add(MailboxAddress.Parse(request.ToMail));
                email.Subject = request.Subject;
                email.Body = request.Body;
                email.IsBodyHtml = true;

                var builder = new BodyBuilder();
                builder.HtmlBody = request.Body;
                //email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient(_host, _port);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_fromMail, _password);

                await smtp.SendMailAsync(email);
                //await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent to {request.ToMail} with purpose {request.Purpose}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email");
                throw;
            }
        }

        private async Task<EmailTemplate> GetEmailTemplateByPurpose(string purpose)
        {
            var template = await _dbContext.EmailTemplates
                        .ToListAsync();

            var filteredTemplate = template.FirstOrDefault(t => t.Purpose.ToString() == purpose.ToString());
            return filteredTemplate!;
        }

        public async Task AddEmailTemplate(EmailTemplate template)
        {
            _dbContext.EmailTemplates.Add(template);
            await _dbContext.SaveChangesAsync();
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