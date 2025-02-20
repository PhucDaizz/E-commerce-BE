using AutoMapper;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Discount;
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
    public class DiscountController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IDiscountRepository discountRepository;
        private readonly IDiscountServices discountServices;

        public DiscountController(IMapper mapper, IDiscountRepository discountRepository, IDiscountServices discountServices)
        {
            this.mapper = mapper;
            this.discountRepository = discountRepository;
            this.discountServices = discountServices;
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateDiscountDTO createDiscountDTO)
        {
            var discounts = mapper.Map<Discounts>(createDiscountDTO);
            var result = await discountRepository.CreateAsync(discounts);
            return Ok(mapper.Map<DiscountDTO>(result));
        }

        [HttpGet]
        [Route("{discountId:int}")]
        public async Task<IActionResult> GetById([FromRoute]int discountId)
        {
            var exsting =  await discountRepository.GetByIdAsync(discountId);
            if (exsting == null)
            {
                return BadRequest("DiscountID is not existing");
            }
            return Ok(exsting);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]int page = 1, [FromQuery]int itemsInPage = 20, [FromQuery]string sortBy = "IsActive", [FromQuery]bool isDESC = false)
        {
            var discountList = await discountRepository.GetAllAsync(page, itemsInPage, sortBy, isDESC);
            if (discountList.Discounts == null || !discountList.Discounts.Any())
            {
                return Ok("Discount is empty!");
            }
            return Ok(discountList);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("Update/{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id,[FromBody]EditDiscountDTO editDiscountDTO)
        {
            var discountUpdate = mapper.Map<Discounts>(editDiscountDTO);
            discountUpdate.DiscountID = id;
            var result = await discountRepository.UpdateAsync(discountUpdate);
            if (result == null)
            {
                return NotFound("DiscountID is not existing");
            }
            return Ok(mapper.Map<DiscountDTO>(result));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var existing = await discountRepository.DeleteAsync(id);
            if (existing == null)
            {
                return NotFound("DiscountID is not existing");
            }
            var result = mapper.Map<DiscountDTO>(existing);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("GetDiscountByCode/{code}/{amount:float}")]
        public async Task<IActionResult> GetDiscountByCode([FromRoute]string code, [FromRoute]float amount)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(userIdClaim.Value);

            var discount = await discountServices.CanUseDiscount(userId, code, amount);
            if (discount == null)
            {
                return NotFound("The discount code does not exist or has already been used.");
            }
            var result = mapper.Map<DiscountDTO>(discount);
            return Ok(result);
        }

    }
}
