using Ecommerce.Application.DTOS.Common;
using Ecommerce.Application.DTOS.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IProductServices
    {
        Task<DetailProductDTO?> GetProductDetailAsync(int id);
        Task<bool> PauseSalesAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusProduct(int productId);
        Task<PagedResult<ListProductDTO>> GetRecommendAsync(int productId, int pageIndex, int pageSize);
    }
}
