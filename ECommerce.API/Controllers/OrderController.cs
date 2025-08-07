using AutoMapper;
using Ecommerce.Application.DTOS.Order;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create([FromBody]CreateOrderDTO createOrderDTO)
        {
            var order = _mapper.Map<Orders>(createOrderDTO);
            
            order = await _orderRepository.CreateAsync(order);
            return Ok(_mapper.Map<OrderDetailDTO>(order));
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("GetOrderDetailById/{orderID:Guid}")]
        public async Task<IActionResult> GetDetailOrder([FromRoute]Guid orderID)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Please login again!.");
            }
            var userId = Guid.Parse(userIdClaim.Value);
            var order = await _orderRepository.GetByIdAsync(orderID, userId);
            if (order == null)
            {
                return NotFound("OrderId is not existing");
            }
            var resut = _mapper.Map<OrderDetailDTO>(order);
            return Ok(resut);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("ListOrders")]
        public async Task<IActionResult> GetListOrdersByUserID()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Please login again!.");
            }
            var userId = Guid.Parse(userIdClaim.Value);
            var listOrders = await _orderRepository.GetAllByUserIdAsync(userId);
            if (listOrders == null)
            {
                return Ok("Your order is empty");
            }
            return Ok(_mapper.Map<IEnumerable<OrderDTO>>(listOrders));
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("GetDetailOderByIdADMIN")]
        public async Task<IActionResult> GetDetailOrderAdmin([FromQuery]Guid orderId)
        {
            var order = await _orderRepository.GetByIdAdminAsync(orderId);
            if (order == null)
            {
                return NotFound("OrderId is not existing");
            }
            var resut = _mapper.Map<GetDetailOrderDTO>(order);
            return Ok(resut);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetListOrder([FromQuery] Guid? userId, [FromQuery] string? sortBy, [FromQuery] bool isDESC = true, [FromQuery] int page = 1, [FromQuery] int itemInPage = 10)
        {
            var orders = await _orderRepository.GetAllAsync(userId, sortBy, isDESC,page,itemInPage);
            return Ok(orders);

        }
    }
}
