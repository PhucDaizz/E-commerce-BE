using AutoMapper;
using Ecommerce.Application.DTOS.Category;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Categories>(categoryDTO);
            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = DateTime.Now;
            category =  await _categoryRepository.CreateAsync(category);
            return Ok(category);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id,[FromBody]EditCategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Categories>(categoryDTO);
            category.CategoryID = id;
            var existing = await _categoryRepository.UpdateAsync(category);
            if(existing == null)
            {
                return NotFound("Id is not existing!");
            }
            var result = _mapper.Map<CategoryDTO>(existing);
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var categories = await _categoryRepository.GetByIdAsync(id);
            if (categories == null)
            {
                return BadRequest("Id is not existing!");
            }
            var result = _mapper.Map<CategoryDTO>(categories);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var existing = await _categoryRepository.DeleteAsync(id);

            if(existing == null)
            {
                return NotFound("Id is not existing!");
            }
            return Ok(existing);
        }
    }
}
