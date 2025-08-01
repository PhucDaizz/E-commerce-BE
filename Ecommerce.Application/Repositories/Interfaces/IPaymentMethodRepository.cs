using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethods> AddAsync(PaymentMethods paymentMethods);

        Task<IEnumerable<PaymentMethods>?> GetAllAsync();
    }
}
