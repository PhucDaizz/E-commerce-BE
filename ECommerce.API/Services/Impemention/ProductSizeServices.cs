using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductSize;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;

namespace ECommerce.API.Services.Impemention
{
    public class ProductSizeServices : IProductSizeServices
    {
        private readonly IProductSizeRepository productSizeRepository;
        private readonly AppDbContext dbContext;

        public ProductSizeServices(IProductSizeRepository productSizeRepository, AppDbContext dbContext)
        {
            this.productSizeRepository = productSizeRepository;
            this.dbContext = dbContext;
        }


        public async Task<ProductSizeResponse> CreateRangeAsync(CreateProductSizesDTO productSizesDTO)
        {
            if (productSizesDTO == null)
            {
                return await Task.FromResult(new ProductSizeResponse
                {
                    message = "Invalid data!"
                });
            }

            var productSizes = new List<ProductSizes>();
            

            foreach (var productColor in productSizesDTO.ProductSizes)
            {
                int productColorID = productColor.Key;

                foreach (var sizeEntry in productColor.Value)
                {
                    string size = sizeEntry.Key;
                    int stock = sizeEntry.Value;

                    var newProductSize = new ProductSizes
                    {
                        ProductColorID = productColorID,
                        Size = size,
                        Stock = stock,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    var isExisting = await productSizeRepository.IsExistAsync(newProductSize.ProductColorID, newProductSize.Size);
                    if (isExisting)
                    {
                        await productSizeRepository.AddAsync(newProductSize);
                    }
                    else
                    {
                        productSizes.Add(newProductSize);
                    }
                }
            }
            await dbContext.ProductSizes.AddRangeAsync(productSizes);
            await dbContext.SaveChangesAsync();

            return await Task.FromResult(new ProductSizeResponse
            {
                message = "Done!"
            });
        }
    }
}
