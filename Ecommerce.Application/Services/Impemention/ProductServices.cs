using AutoMapper;
using Ecommerce.Application.DTOS.Category;
using Ecommerce.Application.DTOS.Product;
using Ecommerce.Application.DTOS.ProductColor;
using Ecommerce.Application.DTOS.ProductImage;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class ProductServices: IProductServices
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductImageServices _productImageServices;

        public ProductServices(IMapper mapper, 
                IUnitOfWork unitOfWork,
                IOrderRepository orderRepository,
                IOrderDetailRepository orderDetailRepository,
                IProductImageServices productImageServices)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _unitOfWork = unitOfWork;
            _productImageServices = productImageServices;
        }

        public async Task<bool> PauseSalesAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var imageDelete = await _productImageServices.retainProductFeaturedImage(id);

                var privateProduct = await _unitOfWork.Products.ToPublicAync(id);

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }

        public async Task<DetailProductDTO?> GetProductDetailAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryID);
            var color = await _unitOfWork.ProductColors.GetProductColorSizeAsync(id);
            var image = await _unitOfWork.ProductImages.GetAllByProductIDAsync(product.ProductID);

            var productMap = _mapper.Map<ProductDTO>(product);
            var categoryMap = _mapper.Map<CategoryDTO>(category);
            var imageMap = _mapper.Map<IEnumerable<ProductImageDTO>>(image);
            var colorMap = _mapper.Map<IEnumerable<ProductColorDTO>>(color);

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
            var isExisting = await _orderDetailRepository.HasProductInAnyOrderAsync(id);
            if (isExisting)
            {
                throw new Exception("Cannot delete this product because it is already in the order.");
            }
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Id is not existing!");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.CartItems.ClearAllByProductIDAsync(id);
                await _unitOfWork.ProductColors.DeleteProductColorSizeAsync(id);
                await _unitOfWork.Products.DeleteAsync(id);
                await _productImageServices.DeleteProductImagesAsync(id);
                /*await productImageRepository.DeleteProductImagesAsync(id);*/

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }


        }

        public async Task<bool> ChangeStatusProduct(int productId)
        {
            var success = await _unitOfWork.Products.ToPublicAync(productId);

            if (!success)
            {
                return false;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
