using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<Tags> GetByIdAsync(int id);
        Task<Tags> GetBySlugAsync(string slug);
        Task<IReadOnlyList<Tags>> GetAllAsync();
        Task<Tags> AddAsync(Tags tag);
        Task<Tags> UpdateAsync(Tags tag);
        Task DeleteAsync(int tagId);

        // Product-Tag Relationship
        Task AddTagToProductAsync(int productId, int tagId);
        Task RemoveTagFromProductAsync(int productId, int tagId);
        Task<IReadOnlyList<Tags>> GetTagsByProductAsync(int productId);
        Task<IReadOnlyList<Products>> GetProductsByTagAsync(int tagId);

        // Advanced Features
        Task<bool> ExistsAsync(string tagName);
        Task<bool> IsInUseAsync(int tagId);
        Task<IReadOnlyList<Tags>> SearchAsync(string keyword);
        Task<IReadOnlyList<Tags>> GetPopularTagsAsync(int count);

        // Bulk Operations
        Task AddTagsToProductAsync(int productId, IEnumerable<int> tagIds);
        Task SyncProductTagsAsync(int productId, IEnumerable<int> tagIds);
    }
}
