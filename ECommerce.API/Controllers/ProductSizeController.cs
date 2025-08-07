using AutoMapper;
using Ecommerce.Application.DTOS.ProductSize;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizeController : ControllerBase
    {
        private readonly IProductSizeRepository _productSizeRepository;
        private readonly IMapper _mapper;
        private readonly IProductSizeServices _productSizeServices;

        public ProductSizeController(IProductSizeRepository productSizeRepository, IMapper mapper, IProductSizeServices productSizeServices)
        {
            _productSizeRepository = productSizeRepository;
            _mapper = mapper;
            _productSizeServices = productSizeServices;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateProductSizeDTO productSizeDTO)
        {
            var productSize = _mapper.Map<ProductSizes>(productSizeDTO);
            productSize.CreatedAt = DateTime.Now;
            productSize.UpdatedAt = DateTime.Now;  
            var createProductSize = await _productSizeRepository.CreateAsync(productSize);
            var result = _mapper.Map<ProductSizeDTO>(createProductSize);
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
            var result = _mapper.Map<ProductSizeDTO>(existing);
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
            var result = _mapper.Map<ProductSizeDTO>(existing);
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
            var result = _mapper.Map<ProductSizeDTO>(existing);
            return Ok(result);
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]EditProductSizeDTO productSizeDTO)
        {
            var productSize = _mapper.Map<ProductSizes>(productSizeDTO);
            productSize.ProductSizeID = id;
            var existing = await _productSizeRepository.UpdateAsync(productSize);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = _mapper.Map<ProductSizeDTO>(existing);
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
            var result = _mapper.Map<IEnumerable<ProductSizeDTO>>(existing);
            return Ok(result);
        }
    }
}
