using AutoMapper;
using Ecommerce.Infrastructure;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Shipping;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingRepository shippingRepository;
        private readonly IMapper mapper;
        private readonly AppDbContext dbContext;
        private readonly IShippingServices shippingServices;

        public ShippingController(IShippingRepository shippingRepository, IMapper mapper, AppDbContext dbContext, IShippingServices shippingServices)
        {
            this.shippingRepository = shippingRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.shippingServices = shippingServices;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]CreateShippingDTO createShippingDTO)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var shipping = mapper.Map<Shippings>(createShippingDTO);

                // call api delivery services in here


                var shippingResult = await shippingRepository.CreateAsync(shipping);

                return Ok(mapper.Map<ShippingDTO>(shippingResult));


            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles ="SuperAdmin, Admin")]
        [Route("{orderId:Guid}")]
        public async Task<IActionResult> Update([FromRoute]Guid orderId, [FromBody]UpdateShippingDTO updateShippingDTO)
        {
            var shipping = await shippingServices.UpdateShippingAfterCreateAsync(orderId, updateShippingDTO);
            if (shipping == null)
            {
                return NotFound("OrderId is not existing");
            }
            return Ok(mapper.Map<ShippingDTO>(shipping));
        }
    }
}
