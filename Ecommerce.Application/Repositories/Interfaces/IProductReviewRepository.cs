using Ecommerce.Application.DTOS.ProductReview;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IProductReviewRepository
    {
        Task<ProductReviews> CreateAsync(ProductReviews productReview);

        Task<IEnumerable<ProductReviewDTO>?> GetAllAsync(int productId);

        Task<ProductReviews?> DeleteAync(int reviewId);
    }
}
