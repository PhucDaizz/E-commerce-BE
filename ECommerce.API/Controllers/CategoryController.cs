using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Category;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(AppDbContext dbContext, IMapper mapper, ICategoryRepository categoryRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCategoryDTO categoryDTO)
        {
            var category = mapper.Map<Categories>(categoryDTO);
            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = DateTime.Now;
            category =  await categoryRepository.CreateAsync(category);
            return Ok(category);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id,[FromBody]EditCategoryDTO categoryDTO)
        {
            var category = mapper.Map<Categories>(categoryDTO);
            category.CategoryID = id;
            var existing = await categoryRepository.UpdateAsync(category);
            if(existing == null)
            {
                return NotFound("Id is not existing!");
            }
            var result = mapper.Map<CategoryDTO>(existing);
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var categories = await categoryRepository.GetByIdAsync(id);
            if (categories == null)
            {
                return BadRequest("Id is not existing!");
            }
            var result = mapper.Map<CategoryDTO>(categories);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await categoryRepository.GetAllAsync();
            return Ok(categories);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var existing = await categoryRepository.DeleteAsync(id);

            if(existing == null)
            {
                return NotFound("Id is not existing!");
            }
            return Ok(existing);
        }
    }
}
