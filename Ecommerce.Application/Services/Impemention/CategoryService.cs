using Ecommerce.Application.DTOS.Category;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Services.Impemention
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageServiceFactory _storageServiceFactory;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IStorageServiceFactory storageServiceFactory)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _storageServiceFactory = storageServiceFactory;
        }
        public async Task<Categories> CreateCategoryAsync(AddCategoryImageCommand command, Stream fileStream)
        {
            if (command.Category == null)
                throw new ArgumentException("Category information is required.");
            if (string.IsNullOrEmpty(command.FileName))
                throw new ArgumentException("File name is required.");
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("Image file is required.");
            string uniqueFileName = Guid.NewGuid().ToString();

            var storeType = command.UseCloudStorage ? StorageType.Cloudinary : StorageType.Local;
            var storeSevice = _storageServiceFactory.GetService(storeType);

            var imageUrl = await storeSevice.SaveFileAsync(
                fileStream,
                command.FileName,
                "catogories",
                uniqueFileName
            );

            command.Category.ImageURL = imageUrl;
            var newCategory = await _unitOfWork.Categories.CreateAsync(command.Category);
            await _unitOfWork.SaveChangesAsync();
            return newCategory;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(category.ImageURL))
            {
                var storageType = category.ImageURL.Contains("cloudinary.com") ? StorageType.Cloudinary : StorageType.Local;
                var storageService = _storageServiceFactory.GetService(storageType);
                await storageService.DeleteFileAsync(category.ImageURL);
            }

            await _unitOfWork.Categories.DeleteAsync(id);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<Categories?> UpdateCategoryAsync(UpdateCategoryCommand command, Stream? fileStream = null)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(command.Categorie.CategoryID);
            if (existingCategory == null)
            {
                throw new ArgumentException("Banner not found.");
            }

            if (command.HasNewImage && fileStream != null && !string.IsNullOrEmpty(command.FileName))
            {
                if (!string.IsNullOrEmpty(existingCategory.ImageURL))
                {
                    var oldStorageType = existingCategory.ImageURL.Contains("cloudinary.com") ? StorageType.Cloudinary : StorageType.Local;
                    var oldStorageService = _storageServiceFactory.GetService(oldStorageType);
                    await oldStorageService.DeleteFileAsync(existingCategory.ImageURL);
                }

                string uniqueFileName = Guid.NewGuid().ToString();
                var storageType = command.UseCloudStorage ? StorageType.Cloudinary : StorageType.Local;
                var storageService = _storageServiceFactory.GetService(storageType);

                var newImageUrl = await storageService.SaveFileAsync(
                    fileStream,
                    command.FileName,
                    "catogories",
                    uniqueFileName
                );

                existingCategory.ImageURL = newImageUrl;
            }

            existingCategory.CategoryName = command.Categorie.CategoryName;
            existingCategory.Description = command.Categorie.Description;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            //_unitOfWork.Categories.UpdateAsync(existingCategory);

            await _unitOfWork.SaveChangesAsync();

            return existingCategory;
        }
    }
}
