using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Repositories.Impemention;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductColorController : ControllerBase
    {
        private readonly ECommerceDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductColorRepository productColorRepository;

        public ProductColorController(ECommerceDbContext dbContext, IMapper mapper, IProductColorRepository productColorRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productColorRepository = productColorRepository;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateProductColorDTO productColorDTO)
        {
            var productColor = mapper.Map<ProductColors>(productColorDTO);
            productColor.CreatedAt = DateTime.Now;
            productColor.UpdatedAt = DateTime.Now;
            var createdProductColor = await productColorRepository.CreateAsync(productColor);
            var result = mapper.Map<ProductColorDTO>(createdProductColor);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var existing =  await productColorRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result =  mapper.Map<ProductColorDTO>(existing);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productColors= await productColorRepository.GetAllAsync();
            var result = mapper.Map<IEnumerable<ProductColorDTO>>(productColors);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeletebyId([FromRoute]int id)
        {
            var existing = await productColorRepository.DeleteAsync(id);
            if (existing == null)
            {
                return NotFound("Id is not existing");
            }
            return Ok(existing);   

        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]EditProductColorDTO productColorDTO)
        {
            var productColor = mapper.Map<ProductColors>(productColorDTO);
            productColor.ProductColorID = id;
            productColor.UpdatedAt = DateTime.Now;
            var existing = await productColorRepository.UpdateAsync(productColor);
            if (existing == null)
            {
                return NotFound("Id is not existing");
            }
            var result = mapper.Map<ProductColorDTO>(existing);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllByProductId/{ProductId:int}")]
        public async Task<IActionResult> GetAllByProduct(int ProductId)
        {
            var existing = await productColorRepository.GetProductColorSizeAsync(ProductId);
            if(existing == null)
            {
                return NotFound("Product not found or does not have any colors.");
            }
            var result = mapper.Map<IEnumerable<ProductColorDTO>>(existing);
            return Ok(result);
        }
    }
}
