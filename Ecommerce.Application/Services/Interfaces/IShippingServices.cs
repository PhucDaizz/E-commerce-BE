using Ecommerce.Application.DTOS.Shipping;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IShippingServices
    {
        Task<Shippings> UpdateShippingAfterCreateAsync(Guid orderId, UpdateShippingDTO shipping);
    }
}
