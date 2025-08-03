using AutoMapper;
using Ecommerce.Application.Common.Utils;
using Ecommerce.Application.DTOS.ProductImage;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Services.Impemention
{
    public class ProductImageServices : IProductImageServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageServiceFactory _storageFactory;
        private readonly IMapper _mapper;

        public ProductImageServices(IUnitOfWork unitOfWork, IStorageServiceFactory storageFactory, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _storageFactory = storageFactory;
            _mapper = mapper;
        }

        public async Task<List<ProductImageDTO>> AddImagesToProductAsync(AddImagesCommand command)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.ProductId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            var productNameSlug = StringUtils.Slugify(product.ProductName);

            var storageType = command.UseCloudStorage ? StorageType.Cloudinary : StorageType.Local;
            var storageService = _storageFactory.GetService(storageType);

            var existingImageCount = await _unitOfWork.ProductImages.CountByProductIdAsync(command.ProductId);
            const int maxImageCount = 5;
            var addedImages = new List<ProductImages>();

            foreach (var (stream, fileName) in command.ImageFiles)
            {
                if (existingImageCount >= maxImageCount) break;

                var imageUrl = await storageService.SaveFileAsync(
                    stream,
                    fileName,
                    $"products", // subFolder
                    $"{productNameSlug}_{command.ProductId}" // fileNamePrefix
                );
                var newImage = new ProductImages
                {
                    ProductID = command.ProductId,
                    ImageURL = imageUrl,
                    IsPrimary = (existingImageCount == 0),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ProductImages.CreateAsync(newImage);
                addedImages.Add(newImage);
                existingImageCount++;
            }

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<List<ProductImageDTO>>(addedImages);
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var imageToDelete = await _unitOfWork.ProductImages.GetByIdAsync(imageId);
            if (imageToDelete == null) return false;

            var storageType = imageToDelete.ImageURL.Contains("cloudinary.com") ? StorageType.Cloudinary : StorageType.Local;
            var storageService = _storageFactory.GetService(storageType);

            await storageService.DeleteFileAsync(imageToDelete.ImageURL);

            await _unitOfWork.ProductImages.DeleteAsync(imageToDelete.ImageID);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductImagesAsync(int productId)
        {
            var images = await GetImagesByProductIdAsync(productId);

            foreach (var image in images)
            {
                if (!string.IsNullOrEmpty(image.ImageURL))
                {
                    await DeleteImageAsync(image.ImageID);
                }
            }

            return true;
        }

        public async Task<List<ProductImageDTO>> GetImagesByProductIdAsync(int productId)
        {
            var productExists = await _unitOfWork.Products.ExistsAsync(productId);
            if (!productExists) throw new KeyNotFoundException("Product not found.");

            var images = await _unitOfWork.ProductImages.GetAllByProductIDAsync(productId);
            return _mapper.Map<List<ProductImageDTO>>(images.OrderBy(x =>  x.IsPrimary).ThenBy(x =>  x.CreatedAt));
        }

        public async Task<bool> retainProductFeaturedImage(int productId)
        {
            var images = await GetImagesByProductIdAsync(productId);
            if (images.Count <= 1)
                return false;

            var imagesToDelete = images.Skip(1).ToList();

            foreach (var image in imagesToDelete)
            {
                if (!string.IsNullOrEmpty(image.ImageURL))
                {
                    await DeleteImageAsync(image.ImageID);
                }
            }

            return true;
        }

    }
}
