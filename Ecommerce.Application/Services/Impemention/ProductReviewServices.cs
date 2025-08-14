using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class ProductReviewServices : IProductReviewServices
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public ProductReviewServices(IProductReviewRepository productReviewRepository, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }
        public async Task<ProductReviews> CreateAsync(ProductReviews productReviews)
        {
            // Check if the product exists
            bool productExists = await _productRepository.ExistsAsync(productReviews.ProductID);
            if (!productExists)
            {
                throw new Exception("Product does not exist");
            }

            // Times buy this product
            int timesBuy = await _orderRepository.GetPurchaseCountAsync(productReviews.UserID, productReviews.ProductID);

            int timesReview = await _productReviewRepository.GetReviewCountByUserAsync(productReviews.UserID, productReviews.ProductID);

            if(timesBuy == 0)
            {
                throw new Exception("You can't review this product because you haven't bought it yet");
            }

            if (timesReview < timesBuy)
            {
                await _productReviewRepository.CreateAsync(productReviews);
                return productReviews;
            }

            throw new Exception("You have reviewed this product");
        }
    }
}
