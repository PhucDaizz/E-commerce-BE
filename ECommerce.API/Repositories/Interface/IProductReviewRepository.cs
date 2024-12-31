using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IProductReviewRepository
    {
        Task<ProductReviews> CreateAsync(ProductReviews productReview);

        Task<IEnumerable<ProductReviews>?> GetAllAsync(int productId);

        Task<ProductReviews?> DeleteAync(int reviewId);
    }
}
