using AutoMapper;
using Ecommerce.Infrastructure;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;
        private readonly IProductServices productServices;

        public ProductController(AppDbContext dbContext, IProductRepository productRepository, IMapper mapper, IProductServices productServices)
        {
            this.dbContext = dbContext;
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.productServices = productServices;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO productDTO)
        {
            var product = mapper.Map<Products>(productDTO);
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product = await productRepository.CreateAsync(product);
            return Ok(product);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var products = await productRepository.GetByIdAsync(id);
            if (products == null)
            {
                return BadRequest("Id is not existing!");
            }
            return Ok(products);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string? productName, [FromQuery] bool isDESC = false, [FromQuery] int page = 1, [FromQuery] int itemInPage = 20, [FromQuery] string sortBy = "CreatedAt", [FromQuery] int? categoryId = null)
        {
            var products = await productRepository.GetAllAsync(productName, isDESC, page, itemInPage, sortBy, categoryId);
            return Ok(products);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] EditProductDTO productDTO)
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

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await productServices.DeleteAsync(id);
                return Ok("Product is deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Detail/{id:int}")]
        public async Task<IActionResult> GetDetailById([FromRoute] int id)
        {
            var existing = await productServices.GetProductDetailAsync(id);
            if (existing == null)
            {
                return BadRequest("Id is not existing!");
            }
            return Ok(existing);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("GetAllAdmin")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] string? productName, [FromQuery] bool isDESC = false, [FromQuery] int page = 1, [FromQuery] int itemInPage = 20, [FromQuery] string sortBy = "CreatedAt", [FromQuery] int? categoryId = null)
        {
            var products = await productRepository.GetAllAdminAsync(productName, isDESC, page, itemInPage, sortBy, categoryId);
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("ToPublic/{productID:int}")]
        public async Task<IActionResult> ToPublic([FromRoute] int productID)
        {
            var result = await productRepository.ToPublicAync(productID);
            if (!result)
            {
                return BadRequest("Id is not existing!");
            }
            return Ok("Product is public");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("pausesale/{productId:int}")]
        public async Task<IActionResult> PauseSale([FromRoute] int productId)
        {
            var result = await productServices.PauseSalesAsync(productId);
            if (!result)
            {
                return BadRequest("Id is not existing!");
            }
            return Ok("Product is paused");
        }
    }
}
