using Ecommerce.Application.DTOS.Category;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Categories> CreateCategoryAsync(AddCategoryImageCommand command, Stream fileStream);
        Task<Categories?> UpdateCategoryAsync(UpdateCategoryCommand command, Stream? fileStream = null);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
