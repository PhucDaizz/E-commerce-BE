using AutoMapper;
using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Models.DTO.ProductImage;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
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

        public ProductServices(IMapper mapper, IProductRepository productRepository, ICategoryRepository categoryRepository, IProductColorRepository productColorRepository, IProductImageRepository productImageRepository)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.productColorRepository = productColorRepository;
            this.productImageRepository = productImageRepository;
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
    }
}
