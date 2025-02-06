using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductReview;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ECommerceDbContext dbContext;
        private readonly AuthDbContext authDbContext;

        public ProductReviewRepository(ECommerceDbContext dbContext, AuthDbContext authDbContext)
        {
            this.dbContext = dbContext;
            this.authDbContext = authDbContext;
        }
        public async Task<ProductReviews> CreateAsync(ProductReviews productReview)
        {
            await dbContext.ProductReviews.AddAsync(productReview);
            await dbContext.SaveChangesAsync();
            return productReview;
        }

        public async Task<ProductReviews?> DeleteAync(int reviewId)
        {
            var exising = await dbContext.ProductReviews.FirstOrDefaultAsync(x => x.ReviewID == reviewId);
            if(exising == null)
            {
                return null;
            }
            dbContext.ProductReviews.Remove(exising);
            await dbContext.SaveChangesAsync();
            return exising;
        }

        public async Task<IEnumerable<ProductReviewDTO>?> GetAllAsync(int productId)
        {
            var productReviews = await dbContext.ProductReviews.Where(x => x.ProductID == productId).OrderByDescending(x => x.CreatedAt).ToListAsync();

            if (!productReviews.Any())
            {
                return null;
            }

            var userIds = productReviews.Select(x => x.UserID.ToString()).Distinct();

            var users = await authDbContext.Users
                                   .Where(user => userIds.Contains(user.Id))
                                   .ToDictionaryAsync(user => user.Id, user => user.UserName);

            var result = productReviews.Select(review => new ProductReviewDTO
            {
                ReviewID = review.ReviewID,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Username = users.ContainsKey(review.UserID.ToString()) ? users[review.UserID.ToString()] : "Unknown"
            });

            return result;
        }
    }
}
