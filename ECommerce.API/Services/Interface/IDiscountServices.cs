﻿using ECommerce.API.Models.Domain;

namespace ECommerce.API.Services.Interface
{
    public interface IDiscountServices
    {
        Task<double> ApplyDiscountAsync(int id, Guid userID, double amount, bool isUpdateData = true);

        Task<Discounts> CanUseDiscount(Guid userId, string code, float amount);
    }
}
