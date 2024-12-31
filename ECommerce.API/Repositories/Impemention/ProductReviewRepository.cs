using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ECommerceDbContext dbContext;

        public ProductReviewRepository(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
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

        public async Task<IEnumerable<ProductReviews>?> GetAllAsync(int productId)
        {
            var productReviews = await dbContext.ProductReviews.Where(x => x.ProductID == productId).OrderByDescending(x => x.CreatedAt).ToListAsync();
            if(!productReviews.Any())
            {
                return null;
            }
            return productReviews;
        }
    }
}
