using System;
using System.Collections;
using System.Threading.Tasks;
using SmartWr.Ipos.Core.Enums;

namespace SmartWr.Ipos.Core.Messaging
{
    public interface IEmailService : IDisposable
    {
        EmailType EmailType { get; set; }

        Task SendMailAsync(string receiver, string sender, string subject, string body, string displayName = null);

        void SendMail(string receiver, string sender, string subject, string body, string displayName = null);

        void SendTemplatedEmail(EmailType emailType, string emailAddress, IDictionary replacements, string subject);
    }
}