using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.Discount;
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
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountServices _discountServices;

        public DiscountController(IDiscountRepository discountRepository, IDiscountServices discountServices)
        {
            _discountRepository = discountRepository;
            _discountServices = discountServices;
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateDiscountDTO createDiscountDTO)
        {
            if (createDiscountDTO.DiscountValue <= 0 || createDiscountDTO.MinOrderValue <= 0)
            {
                return BadRequest("DiscountValue and MinOrderValue must be greater than 0");
            }

            if (createDiscountDTO.DiscountType == 2 && createDiscountDTO.DiscountValue > 100) // Percentage
            {
                return BadRequest("DiscountValue must be less than or equal to 100 for percentage-based discounts");
            }

            if (createDiscountDTO.DiscountType != 1 && createDiscountDTO.DiscountType != 2)
            {
                return BadRequest("Invalid DiscountType. It must be either 1 (FixedAmount) or 2 (Percentage)");
            }

            if (createDiscountDTO.StartDate >= createDiscountDTO.EndDate)
            {
                return BadRequest("StartDate must be less than EndDate");
            }
            var discounts = createDiscountDTO.ToEntity();
            var result = await _discountRepository.CreateAsync(discounts);
            return Ok(result.ToDiscountDTO());
        }

        [HttpGet]
        [Route("{discountId:int}")]
        public async Task<IActionResult> GetById([FromRoute]int discountId)
        {
            var exsting =  await _discountRepository.GetByIdAsync(discountId);
            if (exsting == null)
            {
                return BadRequest("DiscountID is not existing");
            }
            return Ok(exsting.ToDiscountDTO());
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]int page = 1, [FromQuery]int itemsInPage = 20, [FromQuery]string sortBy = "IsActive", [FromQuery]bool isDESC = false)
        {
            var discountList = await _discountRepository.GetAllAsync(page, itemsInPage, sortBy, isDESC);
            if (discountList.Discounts == null || !discountList.Discounts.Any())
            {
                return NotFound("Discount is empty!");
            }
            return Ok(discountList);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("Update/{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id,[FromBody]EditDiscountDTO editDiscountDTO)
        {
            if (editDiscountDTO.DiscountValue <= 0 || editDiscountDTO.MinOrderValue <= 0)
            {
                return BadRequest("DiscountValue and MinOrderValue must be greater than 0");
            }

            if (editDiscountDTO.DiscountType == 2 && editDiscountDTO.DiscountValue > 100) // Percentage
            {
                return BadRequest("DiscountValue must be less than or equal to 100 for percentage-based discounts");
            }

            if (editDiscountDTO.DiscountType != 1 && editDiscountDTO.DiscountType != 2)
            {
                return BadRequest("Invalid DiscountType. It must be either 1 (FixedAmount) or 2 (Percentage)");
            }

            if (editDiscountDTO.StartDate >= editDiscountDTO.EndDate)
            {
                return BadRequest("StartDate must be less than EndDate");
            }
            var discountUpdate = editDiscountDTO.ToEntity();
            discountUpdate.DiscountID = id;
            var result = await _discountRepository.UpdateAsync(discountUpdate);
            if (result == null)
            {
                return NotFound("DiscountID is not existing");
            }
            return Ok(result.ToDiscountDTO());
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var existing = await _discountRepository.DeleteAsync(id);
            if (existing == null)
            {
                return NotFound("DiscountID is not existing or has been use");
            }
            var result = existing.ToDiscountDTO();
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("GetDiscountByCode/{code}/{amount:float}")]
        public async Task<IActionResult> GetDiscountByCode([FromRoute]string code, [FromRoute]float amount)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(userIdClaim.Value);

            var discount = await _discountServices.CanUseDiscount(userId, code, amount);
            if (discount == null)
            {
                return NotFound("The discount code does not exist or has already been used.");
            }
            var result2 = discount.ToDiscountDTO();
            return Ok(result2);
        }

        [HttpPut("ChangeStatus/{id:int}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> changeStatus([FromRoute]int id)
        {
            var discount = await _discountRepository.ActiveAsync(id);
            if (discount == null)
            {
                return NotFound("DiscountID is not existing");
            }
            return Ok(discount.ToDiscountDTO());
        }

    }
}
