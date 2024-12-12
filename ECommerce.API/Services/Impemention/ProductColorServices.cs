using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Models.DTO.ProductSize;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Impemention
{
    public class ProductColorServices : IProductColorServices
    {
        private readonly ECommerceDbContext dbContext;
        private readonly IProductColorRepository productColorRepository;
        private readonly IProductSizeRepository productSizeRepository;

        public ProductColorServices(ECommerceDbContext dbContext, IProductColorRepository productColorRepository, IProductSizeRepository productSizeRepository)
        {
            this.dbContext = dbContext;
            this.productColorRepository = productColorRepository;
            this.productSizeRepository = productSizeRepository;
        }
        
    }
}
