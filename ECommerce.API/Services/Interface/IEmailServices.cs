namespace ECommerce.API.Services.Interface
{
    public interface IEmailServices
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML);
    }
}
