using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductSize;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizeController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IProductSizeRepository productSizeRepository;
        private readonly IMapper mapper;
        private readonly IProductSizeServices productSizeServices;

        public ProductSizeController(AppDbContext dbContext, IProductSizeRepository productSizeRepository, IMapper mapper, IProductSizeServices productSizeServices)
        {
            this.dbContext = dbContext;
            this.productSizeRepository = productSizeRepository;
            this.mapper = mapper;
            this.productSizeServices = productSizeServices;
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

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("AddRange")]
        public async Task<IActionResult> CreateRange([FromBody]CreateProductSizesDTO productSizesDTO)
        {
            var result = await productSizeServices.CreateRangeAsync(productSizesDTO);

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
        [HttpDelete]
        [Route("DeleteByColorAndSize/{colorID:int}")]
        public async Task<IActionResult> DeleteByColorAndSize([FromRoute]int colorID, [FromQuery]string size)
        {
            var existing = await productSizeRepository.DeleteByColorAndSizeAsync(colorID,size);
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
