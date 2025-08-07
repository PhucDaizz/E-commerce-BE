using Ecommerce.Application.DTOS.ProductColor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IProductColorServices
    {
        Task<ProductColorDTO?> DeleteColorAsync(int colorID);
    }
}
