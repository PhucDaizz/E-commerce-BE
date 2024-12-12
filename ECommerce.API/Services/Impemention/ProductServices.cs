using AutoMapper;
using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;

namespace ECommerce.API.Services.Impemention
{
    public class ProductServices : IProductServices
    {
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductColorRepository productColorRepository;

        public ProductServices(IMapper mapper, IProductRepository productRepository, ICategoryRepository categoryRepository, IProductColorRepository productColorRepository)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.productColorRepository = productColorRepository;
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

            var productMap = mapper.Map<ProductDTO>(product);
            var categoryMap = mapper.Map<CategoryDTO>(category);
            /*var colorMap = mapper.Map<IEnumerable<ProductColorDTO>>(color);*/

            return new DetailProductDTO
            {
                Product = productMap,
                Category = categoryMap,
                Color = color
            };
        }
    }
}
