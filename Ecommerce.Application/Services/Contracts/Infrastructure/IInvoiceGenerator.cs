using Ecommerce.Application.DTOS.User;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services.Contracts.Infrastructure
{
    public interface IInvoiceGenerator
    {
        string GenerateInvoiceHtml(Orders orders, InforDTO infor, Discounts? discounts);
    }
}
