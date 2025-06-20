﻿using ECommerce.API.Models.DTO.ProductColor;

namespace ECommerce.API.Services.Interface
{
    public interface IProductColorServices
    {
        Task<ProductColorDTO?> DeleteColorAsync(int colorID);
    }
}
