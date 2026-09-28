using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.ProductColor;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class ProductColorServices : IProductColorServices
    {
        private readonly IProductColorRepository _productColorRepository;
        private readonly IProductSizeRepository _productSizeRepository;

        public ProductColorServices(IProductColorRepository productColorRepository, IProductSizeRepository productSizeRepository)
        {
            _productColorRepository = productColorRepository;
            _productSizeRepository = productSizeRepository;
        }

        public async Task<ProductColorDTO?> DeleteColorAsync(int colorID)
        {
            await _productSizeRepository.DeleteByColorIDAsync(colorID);
            var productColor = await _productColorRepository.DeleteAsync(colorID);
            if (productColor == null)
            {
                return null;
            }
            var result = productColor.ToProductColorDTO();
            return result;
        }
    }
}
