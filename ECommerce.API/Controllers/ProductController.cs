using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ECommerceDbContext dbContext;
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public ProductController(ECommerceDbContext dbContext, IProductRepository productRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateProductDTO productDTO)
        {
            var product = mapper.Map<Products>(productDTO);
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product = await productRepository.CreateAsync(product);
            return Ok(product);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var products = await productRepository.GetByIdAsync(id);
            if (products == null)
            {
                return BadRequest("Id is not existing!");
            }
            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]EditProductDTO productDTO)
        {
            var product = mapper.Map<Products>(productDTO);
            product.ProductID = id;
            product.UpdatedAt = DateTime.Now;
            var existing = await productRepository.UpdateAsync(product);
            if (existing == null)
            {
                return BadRequest("Id is not existing!");
            }
            return Ok(existing);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var product = await productRepository.DeleteAsync(id);
            if(product == null)
            {
                return BadRequest("Id is not existing!");
            }
            return Ok(product);
        }
    }
}
