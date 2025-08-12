using AutoMapper;
using Ecommerce.Application.DTOS.Order;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IOrderServices _orderServices;

        public OrderController(IOrderRepository orderRepository, IMapper mapper, IOrderServices orderServices)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderServices = orderServices;
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

        [Authorize]
        [HttpPut("CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder([FromRoute] string orderId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not authenticated.");

                var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
                var result = await _orderServices.CanncelOrderAsync(orderId, userId, isAdmin);

                return result
                    ? Ok("Order cancelled successfully.")
                    : BadRequest("Order cancellation failed.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
