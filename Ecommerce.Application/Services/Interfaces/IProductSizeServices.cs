using Ecommerce.Application.DTOS.ProductSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IProductSizeServices
    {
        Task<ProductSizeResponse> CreateRangeAsync(CreateProductSizesDTO productSizesDTO);
    }
}
