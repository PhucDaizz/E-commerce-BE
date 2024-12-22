using AutoMapper;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Discount;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IDiscountRepository discountRepository;

        public DiscountController(IMapper mapper, IDiscountRepository discountRepository)
        {
            this.mapper = mapper;
            this.discountRepository = discountRepository;
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
        public async Task<IActionResult> Update([FromBody]EditDiscountDTO editDiscountDTO)
        {
            var discountUpdate = mapper.Map<Discounts>(editDiscountDTO);
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

    }
}
