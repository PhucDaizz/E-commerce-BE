using ECommerce.API.Models.Domain;

namespace ECommerce.API.Services.Interface
{
    public interface IProductReviewServices
    {
        Task<ProductReviews> CreateAsync(ProductReviews productReviews);
    }
}
