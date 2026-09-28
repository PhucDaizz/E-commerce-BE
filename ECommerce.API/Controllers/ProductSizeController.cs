using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.ProductSize;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizeController : ControllerBase
    {
        private readonly IProductSizeRepository _productSizeRepository;
        private readonly IProductSizeServices _productSizeServices;

        public ProductSizeController(IProductSizeRepository productSizeRepository, IProductSizeServices productSizeServices)
        {
            _productSizeRepository = productSizeRepository;
            _productSizeServices = productSizeServices;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateProductSizeDTO productSizeDTO)
        {
            if (productSizeDTO.Stock < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }
            var productSize = productSizeDTO.ToEntity();
            productSize.CreatedAt = DateTime.Now;
            productSize.UpdatedAt = DateTime.Now;  
            var createProductSize = await _productSizeRepository.CreateAsync(productSize);
            var result = createProductSize.ToProductSizeDTO();
            return Ok(result);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("AddRange")]
        public async Task<IActionResult> CreateRange([FromBody]CreateProductSizesDTO productSizesDTO)
        {
            var result = await _productSizeServices.CreateRangeAsync(productSizesDTO);

            if(result.message == "Invalid data!")
            {
                return BadRequest(result.message);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var existing = await _productSizeRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = existing.ToProductSizeDTO();
            return Ok(result);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteById([FromRoute]int id)
        {
            var existing = await _productSizeRepository.DeleteAsync(id);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = existing.ToProductSizeDTO();
            return Ok(result);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("DeleteByColorAndSize/{colorID:int}")]
        public async Task<IActionResult> DeleteByColorAndSize([FromRoute]int colorID, [FromQuery]string size)
        {
            var existing = await _productSizeRepository.DeleteByColorAndSizeAsync(colorID,size);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = existing.ToProductSizeDTO();
            return Ok(result);
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]EditProductSizeDTO productSizeDTO)
        {
            if (productSizeDTO.Stock < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }
            var productSize = productSizeDTO.ToEntity();
            productSize.ProductSizeID = id;
            var existing = await _productSizeRepository.UpdateAsync(productSize);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = existing.ToProductSizeDTO();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllSizeByColor/{id:int}")]
        public async Task<IActionResult> GetAllSizeByColor([FromRoute]int id)
        {
            var existing = await _productSizeRepository.GetAllByColorAsync(id);
            if (!existing.Any())
            {
                return NotFound("ID is not existing!");
            }
            var result = existing.Select(x => x.ToProductSizeDTO());
            return Ok(result);
        }
    }
}
