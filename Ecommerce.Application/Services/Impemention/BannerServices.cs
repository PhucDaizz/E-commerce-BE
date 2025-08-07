using AutoMapper;
using Ecommerce.Application.DTOS.Banner;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Services.Impemention
{
    public class BannerServices : IBannerServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageServiceFactory _storageServiceFactory;
        private readonly IMapper _mapper;

        public BannerServices(IUnitOfWork unitOfWork, IStorageServiceFactory storageServiceFactory, IMapper mapper)
        {   
            _unitOfWork = unitOfWork;
            _storageServiceFactory = storageServiceFactory;
            _mapper = mapper;
        }

        public async Task<Banners> CreateBannerAsync(AddBannerImageCommand command, Stream fileStream)
        {
            if (command.Banner == null)
                throw new ArgumentException("Banner information is required.");
            if (string.IsNullOrEmpty(command.FileName))
                throw new ArgumentException("File name is required.");
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("Image file is required.");

            string uniqueFileName = Guid.NewGuid().ToString();

            var storeType = command.UseCloudStorage? StorageType.Cloudinary : StorageType.Local;
            var storeSevice = _storageServiceFactory.GetService(storeType);

            var imageUrl = await storeSevice.SaveFileAsync(
                fileStream,
                command.FileName,
                "banners",
                uniqueFileName
            );

            command.Banner.ImageUrl = imageUrl;
            var newBanner = await _unitOfWork.Banners.AddAsync(command.Banner);
            await _unitOfWork.SaveChangesAsync();
            return newBanner;
        }

        public async Task<bool> DeleteBannerAsync(int bannerId)
        {
            var banner = await _unitOfWork.Banners.GetByIdAsync(bannerId);
            if (banner == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(banner.ImageUrl))
            {
                var storageType = banner.ImageUrl.Contains("cloudinary.com") ? StorageType.Cloudinary : StorageType.Local;
                var storageService = _storageServiceFactory.GetService(storageType);
                await storageService.DeleteFileAsync(banner.ImageUrl);
            }

            await _unitOfWork.Banners.DeleteAsync(bannerId);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
