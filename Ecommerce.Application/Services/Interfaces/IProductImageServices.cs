using Ecommerce.Application.DTOS.ProductImage;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IProductImageServices
    {
        Task<List<ProductImageDTO>> AddImagesToProductAsync(AddImagesCommand command);
        Task<bool> DeleteImageAsync(int imageId);
        Task<List<ProductImageDTO>> GetImagesByProductIdAsync(int productId);
        Task<bool> retainProductFeaturedImage(int productId);
        Task<bool> DeleteProductImagesAsync(int productId);
    }
}
