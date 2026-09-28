using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.Shipping;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IShippingRepository _shippingRepository;
        private readonly IShippingServices _shippingServices;

        public ShippingController(IShippingRepository shippingRepository, AppDbContext dbContext, IShippingServices shippingServices)
        {
            this.dbContext = dbContext;
            _shippingRepository = shippingRepository;
            _shippingServices = shippingServices;
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

                var shipping = createShippingDTO.ToEntity();

                // call api delivery services in here


                var shippingResult = await _shippingRepository.CreateAsync(shipping);

                return Ok(shippingResult.ToShippingDTO());


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
            var shipping = await _shippingServices.UpdateShippingAfterCreateAsync(orderId, updateShippingDTO);
            if (shipping == null)
            {
                return NotFound("OrderId is not existing");
            }
            return Ok(shipping.ToShippingDTO());
        }
    }
}
