using AutoMapper;
using Ecommerce.Application.DTOS.ProductColor;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductColorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductColorRepository _productColorRepository;
        private readonly IProductColorServices _productColorServices;

        public ProductColorController(IMapper mapper, IProductColorRepository productColorRepository, IProductColorServices productColorServices)
        {
            _mapper = mapper;
            _productColorRepository = productColorRepository;
            _productColorServices = productColorServices;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateProductColorDTO productColorDTO)
        {
            var productColor = _mapper.Map<ProductColors>(productColorDTO);
            productColor.CreatedAt = DateTime.Now;
            productColor.UpdatedAt = DateTime.Now;
            var createdProductColor = await _productColorRepository.CreateAsync(productColor);
            var result = _mapper.Map<ProductColorDTO>(createdProductColor);
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

            var productColors = _mapper.Map<IEnumerable<ProductColors>>(createProductColorDTOs);
            
            foreach (var productColor in productColors)
            {
                productColor.CreatedAt = DateTime.Now;
                productColor.UpdatedAt = DateTime.Now;
            }
            var createProductColors = await _productColorRepository.CreateRangeAsync(productColors);
            var result = _mapper.Map<IEnumerable<ProductColorDTO>>(createProductColors);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var existing =  await _productColorRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("ID is not existing!");
            }
            var result =  _mapper.Map<ProductColorDTO>(existing);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productColors= await _productColorRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<ProductColorDTO>>(productColors);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeletebyId([FromRoute]int id)
        {
            var existing = await _productColorServices.DeleteColorAsync(id);
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
            var productColor = _mapper.Map<ProductColors>(productColorDTO);
            productColor.ProductColorID = id;
            productColor.UpdatedAt = DateTime.Now;
            var existing = await _productColorRepository.UpdateAsync(productColor);
            if (existing == null)
            {
                return NotFound("Id is not existing");
            }
            var result = _mapper.Map<ProductColorDTO>(existing);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllByProductId/{ProductId:int}")]
        public async Task<IActionResult> GetAllByProduct(int ProductId)
        {
            var existing = await _productColorRepository.GetProductColorSizeAsync(ProductId);
            if(existing == null)
            {
                return NotFound("Product not found or does not have any colors.");
            }
            var result = _mapper.Map<IEnumerable<ProductColorDTO>>(existing);
            return Ok(result);
        }
    }
}
