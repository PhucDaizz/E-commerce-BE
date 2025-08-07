using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IDiscountServices
    {
        Task<double> ApplyDiscountAsync(int id, Guid userID, double amount, bool isUpdateData = true);

        Task<Discounts> CanUseDiscount(Guid userId, string code, float amount);
    }
}
