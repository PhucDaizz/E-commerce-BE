using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Contracts.Infrastructure
{
    public interface IEmailServices
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML);
    }
}
