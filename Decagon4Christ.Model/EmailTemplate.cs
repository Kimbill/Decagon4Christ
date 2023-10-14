using Decagon4Christ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decagon4Christ.Model
{
    public class EmailTemplate : BaseEntity
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public EmailPurpose Purpose { get; set; }
        public const string RegistrationLinkPlaceholder = "{registrationLink}";
    }
}
