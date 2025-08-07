using Ecommerce.Application.DTOS.ProductSize;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class ProductSizeServices : IProductSizeServices
    {
        private readonly IProductSizeRepository _productSizeRepository;

        public ProductSizeServices(IProductSizeRepository productSizeRepository)
        {
            _productSizeRepository = productSizeRepository;
        }


        public async Task<ProductSizeResponse> CreateRangeAsync(CreateProductSizesDTO productSizesDTO)
        {
            if (productSizesDTO == null || !productSizesDTO.ProductSizes.Any())
            {
                return new ProductSizeResponse { message = "Invalid data!" };
            }

            var productSizesToUpsert = new List<ProductSizes>();
            var now = DateTime.UtcNow;

            foreach (var productColorEntry in productSizesDTO.ProductSizes)
            {
                int productColorID = productColorEntry.Key;
                foreach (var sizeEntry in productColorEntry.Value)
                {
                    productSizesToUpsert.Add(new ProductSizes
                    {
                        ProductColorID = productColorID,
                        Size = sizeEntry.Key,
                        Stock = sizeEntry.Value,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            await _productSizeRepository.UpsertRangeAsync(productSizesToUpsert);

            return new ProductSizeResponse { message = "Done!" };
        }
    }
}
