using AutoMapper;
using Ecommerce.Infrastructure;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductImage;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductImageRepository productImageRepository;
        private readonly IProductImageServices productImageServices;
        private readonly IProductRepository productRepository;

        public ProductImageController(AppDbContext dbContext, IMapper mapper, IProductImageRepository productImageRepository, IProductImageServices productImageServices, IProductRepository productRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productImageRepository = productImageRepository;
            this.productImageServices = productImageServices;
            this.productRepository = productRepository;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductImageDTO productImageDTO)
        {
            try
            {
                var product = await productRepository.GetDetailAsync(productImageDTO.ProductID);
                if (product == null)
                {
                    return BadRequest("Product ID is not existing");
                }

                else if (productImageDTO.ImageURL?.Length > 10485760)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File size more than 10MB, please upload a maller size file.");
                }
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];


                string createdImageName = null;
                switch(productImageDTO.OnCloud)
                {
                    case true:
                        if (productImageDTO.ImageURL != null || productImageDTO.ImageURL.Length > 0)
                        {
                            createdImageName = await productImageServices.UploadImageCloundinaryAsync(productImageDTO.ImageURL, allowedFileExtentions, product.ProductName, product.Categories.CategoryName.ToString(), "product");
                        }
                        break;
                    case false:
                        if (productImageDTO.ImageURL != null || productImageDTO.ImageURL.Length > 0)
                        {
                            createdImageName = await productImageServices.SaveImageAsync(productImageDTO.ImageURL, allowedFileExtentions,product.ProductName, product.ProductID);
                        }
                        break;
                }

                // map DTO to domain
                var productImage = new ProductImages
                {
                    ProductID = productImageDTO.ProductID,
                    ImageURL = createdImageName,
                    IsPrimary = productImageDTO.IsPrimary,
                    CreatedAt = DateTime.Now,
                };

                var createImage = await productImageRepository.CreateAsync(productImage);
                return Ok(createImage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /*[Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("UploadImageCloud")]
        public async Task<IActionResult> UploadImage(CreateProductImageDTO productImageDTO)
        {
            try
            {
                if (productImageDTO.ImageURL == null || productImageDTO.ImageURL.Length <= 0)
                {
                    return BadRequest("Image file is required.");
                }

                string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];
                var uploadResult = await productImageServices.UploadListImagesCloudinaryAsync(
                    productImageDTO.ImageURL,
                    allowedFileExtensions,
                    "Dai phuc nguyen",
                    null
                );

                return Ok(uploadResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }
        }*/

        
        
        /*[HttpPost]
        [Route("cloudinaryV2")]
        public async Task<IActionResult> UploadImagePath([FromForm] CreateProductImageDTO productImageDTO)
        {
            try
            {
                if (productImageDTO.ImageURL == null || productImageDTO.ImageURL.Length <= 0)
                {
                    return BadRequest("Image file is required.");
                }

                string[] category = ["Quần"];
                string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];
                var uploadResult = await productImageServices.UploadImageCloundinaryAsync(
                    productImageDTO.ImageURL,
                    allowedFileExtensions,
                    "Dai phuc nguyen",
                    category
                );

                return Ok(uploadResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }
        }*/



        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteById([FromRoute] int id)
        {
            var existing = await productImageRepository.DeleteAsync(id);
            if (existing == null)
            {
                return NotFound("ID is not existing");
            }
            var result = await productImageServices.DeleteImage(existing.ImageURL);
            var productImage = mapper.Map<ProductImageDTO>(existing);
            return Ok(productImage);
        }

        [HttpGet]
        [Route("{idProduct:int}")] 
        public async Task<IActionResult> GetAllImgByIDProduct([FromRoute] int idProduct)
        {
            var imageList = await productImageRepository.GetAllByProductIDAsync(idProduct);

            if (imageList.Count() == 0)
            {
                var productExists = await productRepository.ExistsAsync(idProduct); 
                if (!productExists)
                {
                    return NotFound("ID is not existing");
                }

                return Ok("List img is empty"); 
            }
            var result = mapper.Map<IEnumerable<ProductImageDTO>>(imageList);
            return Ok(result); 
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("UploadListImage/{idProduct:int}")]
        public async Task<IActionResult> CreateImages([FromRoute] int idProduct, List<IFormFile> files, bool? onCloud = false)
        {
            int primary = 1;
            var product = await productRepository.GetDetailAsync(idProduct);
            if (product == null)
            {
                return NotFound("ID product is not existing");
            }
            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
            var length = await dbContext.ProductImages.Where(x => x.ProductID == idProduct).CountAsync();

            var imagesList = await productImageServices.SaveImagesAsync(files, allowedFileExtentions, product, product.ProductID, length, onCloud);

            if (imagesList.Count() == 0 || imagesList == null)
            {
                return BadRequest("Empty image");
            }

            List<ProductImages> images = new List<ProductImages>();
            foreach (var image in imagesList)
            {
                if (length > 5)
                {
                    continue;
                }

                var productImage = new ProductImages
                {
                    ProductID = idProduct,
                    ImageURL = image,
                    IsPrimary = (length == 0) ,
                    CreatedAt = DateTime.Now
                };

                images.Add(productImage);
                primary++;
                length++;
            }
            var imageData = await productImageRepository.CreateImagesAsync(images);
            var result = mapper.Map<IEnumerable<ProductImageDTO>>(imageData);

            return Ok(result);
        }
    }
}
