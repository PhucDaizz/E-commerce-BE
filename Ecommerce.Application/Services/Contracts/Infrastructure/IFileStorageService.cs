using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Contracts.Infrastructure
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string originalFileName, string subFolder, string fileNamePrefix);
        Task DeleteFileAsync(string relativePath);
    }
}
