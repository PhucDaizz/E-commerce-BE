using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Ecommerce.Infrastructure.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IOptions<EmailSettings> _options;

        public EmailServices(IOptions<EmailSettings> options)
        {
            _options = options;
        }
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML)
        {
            string MailServer = _options.Value.MailServer;
            string FromEmail = _options.Value.FromEmail;
            string Password = _options.Value.Password;
            int Port = _options.Value.MailPort;

            var smtpClient = new SmtpClient(MailServer, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(FromEmail),
                To = { toEmail },
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHTML
            };
            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
