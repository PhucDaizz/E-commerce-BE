using Ecommerce.Application.DTOS.ProductImage;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using ECommerce.API.Models.DTO.ProductImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageServices _productImageServices;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageController(IProductImageServices productImageServices, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productImageServices = productImageServices;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductImageDTO productImageDTO)
        {

            try
            {
                var product = await _productRepository.GetDetailAsync(productImageDTO.ProductID);
                if (product == null)
                {
                    return BadRequest("Product ID is not existing");
                }

                if (productImageDTO.ImageURL == null || productImageDTO.ImageURL.Length == 0)
                {
                    return BadRequest("Image file is required.");
                }

                if (productImageDTO.ImageURL.Length > 10485760)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File size more than 10MB, please upload a smaller file.");
                }

                string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];
                var ext = Path.GetExtension(productImageDTO.ImageURL.FileName).ToLowerInvariant();
                if (!allowedFileExtensions.Contains(ext))
                {
                    return BadRequest($"Only {string.Join(", ", allowedFileExtensions)} are allowed.");
                }

                var command = new AddImagesCommand
                {
                    ProductId = productImageDTO.ProductID,
                    UseCloudStorage = productImageDTO.OnCloud,
                    ImageFiles = new List<(Stream, string)>
                    {
                        (productImageDTO.ImageURL.OpenReadStream(), productImageDTO.ImageURL.FileName)
                    }
                };

                var result = await _productImageServices.AddImagesToProductAsync(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteById([FromRoute] int id)
        {

            var success = await _productImageServices.DeleteImageAsync(id);
            if (!success)
            {
                return NotFound("Image not found.");
            }
            return NoContent();
        }

        [HttpGet]
        [Route("{idProduct:int}")] 
        public async Task<IActionResult> GetAllImgByIDProduct([FromRoute] int idProduct)
        {
            try
            {
                var images = await _productImageServices.GetImagesByProductIdAsync(idProduct);
                return Ok(images);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("UploadListImage/{idProduct:int}")]
        public async Task<IActionResult> CreateImages([FromRoute] int idProduct, List<IFormFile> files, bool onCloud = false)
        {
            if (files == null || !files.Any()) return BadRequest("No files provided.");

            var command = new AddImagesCommand
            {
                ProductId = idProduct,
                UseCloudStorage = onCloud,
                ImageFiles = files.Select(f => (f.OpenReadStream(), f.FileName)).ToList()
            };

            try
            {
                var result = await _productImageServices.AddImagesToProductAsync(command);
                // Trả về kết quả tương tự như trước đây: danh sách các DTO của ảnh đã tạo
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
