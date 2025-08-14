using AutoMapper;
using Ecommerce.Application.DTOS.Tag;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        [Route("GetById/{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag == null)
                {
                    return NotFound("Tag not found");
                }
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }


        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagDTO tagDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var tag = _mapper.Map<Tags>(tagDTO);
                var result = await _tagRepository.AddAsync(tag);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut]
        [Route("Update/{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id,[FromBody]CreateTagDTO tagDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var tag = _mapper.Map<Tags>(tagDTO);
                tag.TagID = id;
                var tagUpdate  = await _tagRepository.UpdateAsync(tag);
                return Ok(tagUpdate);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _tagRepository.DeleteAsync(id);
                return Ok("Tag deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tags = await _tagRepository.GetAllAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        [Route("GetProductsByTag/{id:int}")]
        public async Task<IActionResult> GetProductsByTag(int id)
        {
            try
            {
                var products = await _tagRepository.GetProductsByTagAsync(id);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        [Route("GetTagsByProduct/{id:int}")]
        public async Task<IActionResult> GetTagsByProduct(int id)
        {
            try
            {
                var tags = await _tagRepository.GetTagsByProductAsync(id);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }

        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [Route("products/{productId}/AddTagsToProduct")]
        public async Task<IActionResult> AddTagsToProduct([FromRoute] int productId, [FromBody] IEnumerable<int> tagIds)
        {
            try
            {
                await _tagRepository.AddTagsToProductAsync(productId, tagIds);
                return Ok("Tags added to product successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut]
        [Route("products/{productId}/SyncProductTags")]
        public async Task<IActionResult> SyncProductTags([FromRoute] int productId, [FromBody] IEnumerable<int> tagIds)
        {
            try
            {
                await _tagRepository.SyncProductTagsAsync(productId, tagIds);
                return Ok("Product tags synchronized successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }

        }



    }
}
