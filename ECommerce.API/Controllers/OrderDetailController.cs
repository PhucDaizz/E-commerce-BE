using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.OrderDetail;
using Ecommerce.Application.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailRepository orderDetailRepository;

        public OrderDetailController(IOrderDetailRepository orderDetailRepository)
        {
            this.orderDetailRepository = orderDetailRepository;
        }

        [HttpGet("getdetail/{orderID}")]
        [Authorize]
        public async Task<IActionResult> GetListOrderDetailsAsync([FromRoute]Guid orderID)
        {
            var orderDetails = await orderDetailRepository.GetListOrderDetailsAsync(orderID);
            if (orderDetails == null)
            {
                return NotFound("OrderId is not existing!");
            }
            return Ok(orderDetails.Select(x => x.ToGetOrderDetailDTO()));
        }

    }
}
