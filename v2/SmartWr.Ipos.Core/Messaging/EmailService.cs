using System;
using System.Collections;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.Utilities;

namespace SmartWr.Ipos.Core.Messaging
{
    public class EmailService : IEmailService, IIdentityMessageService
    {
        private SmtpClient _smtpClient;
        private bool _disposed;

        public Task SendAsync(IdentityMessage message)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(IposConfig.AutomatedFromEmail, IposConfig.AppName);
            mail.To.Add(message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            return _smtpClient.SendMailAsync(mail);
        }

        public EmailType EmailType
        {
            get;
            set;
        }

        public EmailService()
        {
            EmailType = EmailType.None;
            _smtpClient = IposConfig.GetSmtpClient();
        }

        void IEmailService.SendMail(string receiver, string sender, string subject, string body, string displayName)
        {
            SendMail(receiver, sender, subject, body, displayName);
        }

        protected virtual void SendMail(string receiver, string sender, string subject, string body, string displayName)
        {
            if (string.IsNullOrEmpty(receiver))
                throw new ArgumentNullException("Receipient Email-address is null");

            if (string.IsNullOrEmpty(sender))
                throw new ArgumentNullException("Senders Email-address is null");

            MailAddress sendermailAddress = new MailAddress(sender, displayName);
            MailAddress receivermailAddress = new MailAddress(receiver);

            MailMessage message = new MailMessage(sendermailAddress, receivermailAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            _smtpClient.Send(message);
        }

        Task IEmailService.SendMailAsync(string receiver, string sender, string subject, string body, string displayName)
        {
            return SendMailAsync(receiver, sender, subject, body, displayName);
        }

        protected virtual Task SendMailAsync(string receiver, string sender, string subject, string body, string displayName)
        {
            if (string.IsNullOrEmpty(receiver))
                throw new ArgumentNullException("Receipient Email-address is null");

            if (string.IsNullOrEmpty(sender))
                throw new ArgumentNullException("Senders Email-address is null");

            MailAddress sendermailAddress = new MailAddress(sender, displayName);
            MailAddress receivermailAddress = new MailAddress(receiver);

            MailMessage message = new MailMessage(sendermailAddress, receivermailAddress);
            message.Subject = subject;
            message.Body = body;

            try
            {
                return _smtpClient.SendMailAsync(message);
            }
            catch (Exception)
            {
                return Task.FromResult<int>(0);
            }
        }

        private string GetTemplate()
        {
            switch (EmailType)
            {
                case EmailType.AccountActivation:
                    return Extension.GenerateEmailPath(AppKeys.AccountActivationTemplate);

                case EmailType.StockReorder:
                    return Extension.GenerateEmailPath(AppKeys.StockReorderTemplate);

                case EmailType.Registration:
                    return Extension.GenerateEmailPath(AppKeys.RegistrationTemplate);

                case EmailType.PasswordReset:
                    return Extension.GenerateEmailPath(AppKeys.PasswordResetTemplate);

                case EmailType.Locked:
                    return Extension.GenerateEmailPath(AppKeys.AccountLockedTemplate);

                case EmailType.None:
                default:
                    return string.Empty;
            }
        }

        protected virtual void SendTemplatedEmail(
            string receiver,
            string sender,
            string subject,
            string displayName,
            IDictionary replacements
            )
        {
            var md = new MailDefinition();

            string templatePath = GetTemplate();
            md.BodyFileName = templatePath;
            md.IsBodyHtml = true;
            md.Subject = subject;
            md.From = sender;
            var cntrl = new Control();
            var message = md.CreateMailMessage(receiver, replacements, cntrl);
            message.From = new MailAddress(sender, displayName);
            try
            {
                Task.Run(() =>
                {
                    _smtpClient.Send(message);
                }).ContinueWith((task) =>
                {
                    if (task.IsFaulted)
                        throw task.Exception;
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        void IEmailService.SendTemplatedEmail(EmailType emailType, string emailAddress, IDictionary replacements, string subject)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException("receipient Email-address");

            EmailType = emailType;

            SendTemplatedEmail(
                emailAddress, IposConfig.AutomatedFromEmail,
               subject ?? EmailType.ToString()
                , IposConfig.AppName, replacements
                );
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
                if (_smtpClient != null)
                    _smtpClient.Dispose();
            _disposed = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}