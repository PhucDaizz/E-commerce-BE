using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Infrastructure.Services
{
    public class StorageServiceFactory : IStorageServiceFactory
    {
        private readonly LocalFileStorageService _localFileStorageService;
        private readonly CloudinaryImageStorageService _cloudinaryImageStorageService;

        public StorageServiceFactory(LocalFileStorageService localFileStorageService, CloudinaryImageStorageService cloudinaryImageStorageService)
        {
            _localFileStorageService = localFileStorageService;
            _cloudinaryImageStorageService = cloudinaryImageStorageService;
        }
        public IFileStorageService GetService(StorageType storageType)
        {
            return storageType switch
            {
                StorageType.Cloudinary => _cloudinaryImageStorageService,
                StorageType.Local => _localFileStorageService,
                _ => throw new NotSupportedException($"Storage type '{storageType}' is not supported.")
            };
        }
    }
}
