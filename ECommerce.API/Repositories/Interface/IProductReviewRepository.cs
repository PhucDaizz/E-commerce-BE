using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductReview;

namespace ECommerce.API.Repositories.Interface
{
    public interface IProductReviewRepository
    {
        Task<ProductReviews> CreateAsync(ProductReviews productReview);

        Task<IEnumerable<ProductReviewDTO>?> GetAllAsync(int productId);

        Task<ProductReviews?> DeleteAync(int reviewId);
    }
}
