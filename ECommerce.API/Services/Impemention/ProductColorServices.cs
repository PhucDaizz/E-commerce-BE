using AutoMapper;
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
        private readonly IMapper mapper;

        public ProductColorServices(ECommerceDbContext dbContext, IProductColorRepository productColorRepository, IProductSizeRepository productSizeRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.productColorRepository = productColorRepository;
            this.productSizeRepository = productSizeRepository;
            this.mapper = mapper;
        }

        public async Task<ProductColorDTO?> DeleteColorAsync(int colorID)
        {
            await productSizeRepository.DeleteByColorIDAsync(colorID);
            var productColor = await productColorRepository.DeleteAsync(colorID);
            if (productColor == null)
            {
                return null;
            }
            var result = mapper.Map<ProductColorDTO>(productColor);
            return result;
        }
    }
}
