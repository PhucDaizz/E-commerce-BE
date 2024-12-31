using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.ProductReview;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly ECommerceDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductReviewServices productReviewServices;
        private readonly IProductReviewRepository productReviewRepository;

        public ProductReviewController(ECommerceDbContext dbContext, IMapper mapper, IProductReviewServices productReviewServices, IProductReviewRepository productReviewRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productReviewServices = productReviewServices;
            this.productReviewRepository = productReviewRepository;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateProductReview([FromBody]CreateProductReviewDTO createProductReviewDTO)
        {
            try
            {

                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }
                var useridClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (useridClaim == null)
                {
                    return Unauthorized("Please login again!.");
                }

                var productReview = mapper.Map<ProductReviews>(createProductReviewDTO);
                productReview.CreatedAt = DateTime.Now;
                productReview.UpdatedAt = DateTime.Now;
                productReview.UserID = Guid.Parse(useridClaim.Value);
                
                var productReviewResult = await productReviewServices.CreateAsync(productReview);

                return Ok(mapper.Map<ProductReviewDTO>(productReviewResult));
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllByProductID([FromRoute]int productID)
        {
            try
            {
                var productReviews = await productReviewRepository.GetAllAsync(productID);

                return Ok(mapper.Map<IEnumerable<ProductReviewDTO>>(productReviews));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteByReviewID([FromRoute]int reviewID)
        {
            try
            {
                var productReviews = await productReviewRepository.DeleteAync(reviewID);
                if (productReviews == null)
                {
                    return NotFound("Can't find this review");
                }
                return Ok(mapper.Map<ProductReviewDTO>(productReviews));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
