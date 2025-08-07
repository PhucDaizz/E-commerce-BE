using Ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Contracts.Infrastructure
{
    public interface IStorageServiceFactory
    {
        IFileStorageService GetService(StorageType storageType);
    }
}
