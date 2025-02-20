using ECommerce.API.Services.Interface;
using System.Net;
using System.Net.Mail;

namespace ECommerce.API.Services.Impemention
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration configuration;

        public EmailServices(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML)
        {
            string MailServer = configuration["EmailSettings:MailServer"];
            string FromEmail = configuration["EmailSettings:FromEmail"];
            string Password = configuration["EmailSettings:Password"];
            int Port = int.Parse(configuration["EmailSettings:MailPort"]);

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
