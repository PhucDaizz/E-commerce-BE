using AutoMapper;
using Ecommerce.Infrastructure;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductColor;
using ECommerce.API.Repositories.Impemention;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductColorController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductColorRepository productColorRepository;
        private readonly IProductColorServices productColorServices;

        public ProductColorController(AppDbContext dbContext, IMapper mapper, IProductColorRepository productColorRepository, IProductColorServices productColorServices)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productColorRepository = productColorRepository;
            this.productColorServices = productColorServices;
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

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("AddRange")]
        public async Task<IActionResult> CreateRange([FromBody] IEnumerable<CreateProductColorDTO> createProductColorDTOs)
        {
            var productId = createProductColorDTOs.FirstOrDefault().ProductID;
            foreach (var productColor in createProductColorDTOs)
            {
                if (productColor.ProductID != productId || productColor.ProductID == null)
                {
                    return BadRequest("ProductID is not the same! or ProductID is null");
                }
            }

            var productColors = mapper.Map<IEnumerable<ProductColors>>(createProductColorDTOs);
            
            foreach (var productColor in productColors)
            {
                productColor.CreatedAt = DateTime.Now;
                productColor.UpdatedAt = DateTime.Now;
            }
            var createProductColors = await productColorRepository.CreateRangeAsync(productColors);
            var result = mapper.Map<IEnumerable<ProductColorDTO>>(createProductColors);
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
            var existing = await productColorServices.DeleteColorAsync(id);
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
