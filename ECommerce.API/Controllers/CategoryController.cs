using AutoMapper;
using Ecommerce.Application.DTOS.Banner;
using Ecommerce.Application.DTOS.Category;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Impemention;
using Ecommerce.Application.Services.Interfaces;
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
        private readonly ICategoryService _categoryService;

        public CategoryController(IMapper mapper, ICategoryRepository categoryRepository, ICategoryService categoryService)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        /*[Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Categories>(categoryDTO);
            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = DateTime.Now;
            category =  await _categoryRepository.CreateAsync(category);
            return Ok(category);
        }*/

        /*[Authorize(Roles = "Admin, SuperAdmin")]
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
        }*/

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
            /*var existing = await _categoryRepository.DeleteAsync(id);

            if(existing == null)
            {
                return NotFound("Id is not existing!");
            }
            return Ok(existing);
*/
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    return Ok("Category deleted successfully.");
                }
                return NotFound("Category not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting Category: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm]CategoryFormModel model)
        {
            if(model.ImageFile == null || model.ImageFile == null)
            {
                var category = await _categoryRepository.CreateAsync(new Categories
                {
                    CategoryName = model.CategoryName,
                    Description = model.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

                return Ok(category);
            }
            else
            {
                string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];
                var ext = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                if (!allowedFileExtensions.Contains(ext))
                {
                    return BadRequest($"Only {string.Join(", ", allowedFileExtensions)} are allowed.");
                }

                try
                {
                    var command = new AddCategoryImageCommand
                    {
                        Category = new Categories
                        {
                            CategoryName = model.CategoryName,
                            Description = model.Description,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        FileName = model.ImageFile.FileName,
                        UseCloudStorage = model.UseCloudStorage
                    };

                    using var stream = model.ImageFile.OpenReadStream();
                    var result = await _categoryService.CreateCategoryAsync(command, stream);
                    return Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error occurred while creating the banner.");
                }

            }
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateCategory([FromRoute]int id, [FromForm]UpdateCategoryFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input.");
            }

            if (model.ImageFile != null)
            {
                string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];
                var ext = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                if (!allowedFileExtensions.Contains(ext))
                {
                    return BadRequest($"Only {string.Join(", ", allowedFileExtensions)} are allowed.");
                }
            }

            try
            {
                var command = new UpdateCategoryCommand
                {
                    Categorie = new Categories
                    {
                        CategoryID = id,
                        CategoryName = model.CategoryName,
                        Description = model.Description,
                        UpdatedAt = DateTime.UtcNow
                    },
                    FileName = model.ImageFile?.FileName,
                    UseCloudStorage = model.UseCloudStorage,
                    HasNewImage = model.ImageFile != null
                };

                using var stream = model.ImageFile?.OpenReadStream();
                var result = await _categoryService.UpdateCategoryAsync(command, stream);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
