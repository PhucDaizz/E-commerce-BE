using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Infrastructure.Services
{
    public class StorageServiceFactory : IStorageServiceFactory
    {
        private readonly IEnumerable<IFileStorageService> _services;

        public StorageServiceFactory(IEnumerable<IFileStorageService> services)
        {
            _services = services;
        }
        public IFileStorageService GetService(StorageType storageType)
        {
            return storageType switch
            {
                StorageType.Cloudinary => _services.OfType<CloudinaryImageStorageService>().First(),
                StorageType.Local => _services.OfType<LocalFileStorageService>().First(),
                _ => throw new NotSupportedException("Storage type not supported.")
            };
        }
    }
}
