using AutoMapper;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Shipping;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingRepository shippingRepository;
        private readonly IMapper mapper;

        public ShippingController(IShippingRepository shippingRepository, IMapper mapper)
        {
            this.shippingRepository = shippingRepository;
            this.mapper = mapper;
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

    }
}
