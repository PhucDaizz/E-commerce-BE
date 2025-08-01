using Ecommerce.Application.DTOS.Shipping;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IShippingRepository
    {
        Task<Shippings> CreateAsync(Shippings shipping);
        Task<Shippings?> UpdateAsync(Guid orderId, UpdateShippingDTO shipping);
    }
}
