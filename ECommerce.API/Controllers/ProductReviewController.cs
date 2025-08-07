using AutoMapper;
using Ecommerce.Application.DTOS.ProductReview;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductReviewServices _productReviewServices;
        private readonly IProductReviewRepository _productReviewRepository;

        public ProductReviewController(IMapper mapper, IProductReviewServices productReviewServices, IProductReviewRepository productReviewRepository)
        {
            _mapper = mapper;
            _productReviewServices = productReviewServices;
            _productReviewRepository = productReviewRepository;
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

                var productReview = _mapper.Map<ProductReviews>(createProductReviewDTO);
                productReview.CreatedAt = DateTime.Now;
                productReview.UpdatedAt = DateTime.Now;
                productReview.UserID = Guid.Parse(useridClaim.Value);
                
                var productReviewResult = await _productReviewServices.CreateAsync(productReview);

                return Ok(_mapper.Map<ProductReviewDTO>(productReviewResult));
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpGet]
        [Route("{productID:int}")]
        public async Task<IActionResult> GetAllByProductID([FromRoute]int productID)
        {
            try
            {
                var productReviews = await _productReviewRepository.GetAllAsync(productID);

                return Ok(productReviews);
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
                var productReviews = await _productReviewRepository.DeleteAync(reviewID);
                if (productReviews == null)
                {
                    return NotFound("Can't find this review");
                }
                return Ok(_mapper.Map<ProductReviewDTO>(productReviews));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
