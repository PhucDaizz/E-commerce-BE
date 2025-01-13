using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductSize;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizeController : ControllerBase
    {
        private readonly ECommerceDbContext dbContext;
        private readonly IProductSizeRepository productSizeRepository;
        private readonly IMapper mapper;

        public ProductSizeController(ECommerceDbContext dbContext, IProductSizeRepository productSizeRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.productSizeRepository = productSizeRepository;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateProductSizeDTO productSizeDTO)
        {
            var productSize = mapper.Map<ProductSizes>(productSizeDTO);
            productSize.CreatedAt = DateTime.Now;
            productSize.UpdatedAt = DateTime.Now;  
            var createProductSize = await productSizeRepository.CreateAsync(productSize);
            var result = mapper.Map<ProductSizeDTO>(createProductSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var existing = await productSizeRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = mapper.Map<ProductSizeDTO>(existing);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteById([FromRoute]int id)
        {
            var existing = await productSizeRepository.DeleteAsync(id);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = mapper.Map<ProductSizeDTO>(existing);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]EditProductSizeDTO productSizeDTO)
        {
            var productSize = mapper.Map<ProductSizes>(productSizeDTO);
            productSize.ProductSizeID = id;
            var existing = await productSizeRepository.UpdateAsync(productSize);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result = mapper.Map<ProductSizeDTO>(existing);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllSizeByColor/{id:int}")]
        public async Task<IActionResult> GetAllSizeByColor([FromRoute]int id)
        {
            var existing = await productSizeRepository.GetAllByColorAsync(id);
            if (!existing.Any())
            {
                return NotFound("ID is not existing!");
            }
            var result = mapper.Map<IEnumerable<ProductSizeDTO>>(existing);
            return Ok(result);
        }
    }
}
