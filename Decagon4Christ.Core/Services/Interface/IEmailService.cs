using Decagon4Christ.Model;
using Decagon4Christ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decagon4Christ.Core.Services.Interface
{
    public class MailRequest
    {
        public string ToMail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty ;
    }

    public interface IEmailService
    {
        Task SendMail(UserAction userAction, string userEmail);
        Task SendEmailAsync(string toEmail, string subject, string content);
    }

}
