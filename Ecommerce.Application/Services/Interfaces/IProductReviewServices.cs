using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IProductReviewServices
    {
        Task<ProductReviews> CreateAsync(ProductReviews productReviews);
    }
}
