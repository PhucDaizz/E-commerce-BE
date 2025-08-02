using Ecommerce.Application.DTOS.ProductReview;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductReviewRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ProductReviews> CreateAsync(ProductReviews productReview)
        {
            await _dbContext.ProductReviews.AddAsync(productReview);
            await _dbContext.SaveChangesAsync();
            return productReview;
        }

        public async Task<ProductReviews?> DeleteAync(int reviewId)
        {
            var exising = await _dbContext.ProductReviews.FirstOrDefaultAsync(x => x.ReviewID == reviewId);
            if (exising == null)
            {
                return null;
            }
            _dbContext.ProductReviews.Remove(exising);
            await _dbContext.SaveChangesAsync();
            return exising;
        }

        public async Task<IEnumerable<ProductReviewDTO>?> GetAllAsync(int productId)
        {
            var productReviews = await _dbContext.ProductReviews.Where(x => x.ProductID == productId).OrderByDescending(x => x.CreatedAt).ToListAsync();

            if (!productReviews.Any())
            {
                return null;
            }

            var userIds = productReviews.Select(x => x.UserID.ToString()).Distinct();

            var users = await _dbContext.Users
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

        public async Task<int> GetReviewCountByUserAsync(Guid userId, int productId)
        {
            return await _dbContext.ProductReviews
                .CountAsync(pr => pr.ProductID == productId && pr.UserID == userId);
        }
    }
}
