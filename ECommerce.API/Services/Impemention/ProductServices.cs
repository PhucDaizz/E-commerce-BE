using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Models.DTO.ProductImage;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ECommerce.API.Services.Impemention
{
    public class ProductServices : IProductServices
    {
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductColorRepository productColorRepository;
        private readonly IProductImageRepository productImageRepository;
        private readonly AppDbContext _dbContext;
        private readonly IProductSizeRepository _productSizeRepository;
        private readonly ICartItemRepository _cartItemRepository;

        public ProductServices(IMapper mapper, IProductRepository productRepository, ICategoryRepository categoryRepository, IProductColorRepository productColorRepository, IProductImageRepository productImageRepository, AppDbContext dbContext, IProductSizeRepository productSizeRepository, ICartItemRepository cartItemRepository)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.productColorRepository = productColorRepository;
            this.productImageRepository = productImageRepository;
            _dbContext = dbContext;
            _productSizeRepository = productSizeRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<bool> PauseSalesAsync(int id)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var imageDelete = await productImageRepository.retainProductFeaturedImage(id);

                var privateProduct = await productRepository.ToPublicAync(id);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<DetailProductDTO?> GetProductDetailAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            var category = await categoryRepository.GetByIdAsync(product.CategoryID);
            var color = await productColorRepository.GetProductColorSizeAsync(id);
            var image = await productImageRepository.GetAllByProductIDAsync(product.ProductID);

            var productMap = mapper.Map<ProductDTO>(product);
            var categoryMap = mapper.Map<CategoryDTO>(category);
            var imageMap = mapper.Map<IEnumerable<ProductImageDTO>>(image);
            var colorMap = mapper.Map<IEnumerable<ProductColorDTO>>(color);

            return new DetailProductDTO
            {
                Product = productMap,
                Category = categoryMap,
                Color = colorMap,
                Images = imageMap
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var isExisting = await _dbContext.OrderDetails.AnyAsync(x => x.ProductID == id);
            if (isExisting)
            {
                throw new Exception("Cannot delete this product because it is already in the order.");
            }
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
            {
                 throw new Exception("Id is not existing!");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                await _cartItemRepository.ClearAllByProductIDAsync(id);
                await productColorRepository.DeleteProductColorSizeAsync(id);
                await productImageRepository.DeleteProductImagesAsync(id);
                await productRepository.DeleteAsync(id);

                await transaction.CommitAsync();
                return true;
            }
                catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            

        }
    }
}
