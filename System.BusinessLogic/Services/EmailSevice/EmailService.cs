using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Services.EmailSevice
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
        }

        public async Task SendEmailAsync(string toEmail, Email email)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName);
            message.To.Add(toEmail);
            message.Subject = email.Subject;
            message.Body = email.Body;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password)
            };

            await client.SendMailAsync(message);
        }
    }
}
