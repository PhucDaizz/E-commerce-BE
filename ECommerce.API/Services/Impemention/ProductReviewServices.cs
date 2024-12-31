using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Impemention
{
    public class ProductReviewServices : IProductReviewServices
    {
        private readonly IProductReviewRepository productReviewRepository;
        private readonly ECommerceDbContext dbContext;

        public ProductReviewServices(IProductReviewRepository productReviewRepository, ECommerceDbContext dbContext)
        {
            this.productReviewRepository = productReviewRepository;
            this.dbContext = dbContext;
        }
        public async Task<ProductReviews> CreateAsync(ProductReviews productReviews)
        {
            // Check if the product exists
            bool productExists = dbContext.Products.Any(x => x.ProductID == productReviews.ProductID);
            if(!productExists)
            {
                throw new Exception("Product does not exist");
            }


            // Times buy this product
            int timesBuy = await dbContext.OrderDetails
                .Where(od => od.ProductID == productReviews.ProductID &&
                dbContext.Orders
                .Where(o => o.UserID == productReviews.UserID)
                .Select(o => o.OrderID)
                .Contains(od.OrderID))
            .CountAsync();
                
            int timesReview = await dbContext.ProductReviews
                .Where(pr => pr.ProductID == productReviews.ProductID && pr.UserID == productReviews.UserID)
                .CountAsync();

            if(timesReview < timesBuy)
            {
                await productReviewRepository.CreateAsync(productReviews);  
                return productReviews;
            }

            throw new Exception("You can't review this product");
        }
    }
}
