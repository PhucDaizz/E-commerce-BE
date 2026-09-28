using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.Order;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderServices _orderServices;
        private readonly IAuthRepository _authRepository;
        private readonly IInvoiceGenerator _invoiceGenerator;
        private readonly IDiscountRepository _discountRepository;

        public OrderController(IOrderRepository orderRepository, IOrderServices orderServices, IAuthRepository authRepository, IInvoiceGenerator invoiceGenerator, IDiscountRepository discountRepository)
        {
            _orderRepository = orderRepository;
            _orderServices = orderServices;
            _authRepository = authRepository;
            _invoiceGenerator = invoiceGenerator;
            _discountRepository = discountRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create([FromBody]CreateOrderDTO createOrderDTO)
        {
            var order = createOrderDTO.ToEntity();
            
            order = await _orderRepository.CreateAsync(order);
            return Ok(order.ToOrderDetailDTO());
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
            var resut = order.ToOrderDetailDTO();
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
            return Ok(listOrders.Select(x => x.ToOrderDTO()));
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
            var resut = order.ToGetDetailOrderDTO();
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

        [HttpGet("{orderId}/html")]
        public async Task<IActionResult> GetInvoiceHtml(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAdminAsync(orderId);
                if (order == null)
                    return NotFound($"Order with ID {orderId} not found.");

                var userInfo = await _authRepository.GetInforAsync(order.UserID.ToString());
                if (userInfo == null)
                    return NotFound($"User information not found for order {orderId}");

                Discounts? discount = null;
                if (order.DiscountID != null)
                {
                    discount = await _discountRepository.GetByIdAsync(order.DiscountID.Value);
                }

                var htmlContent = _invoiceGenerator.GenerateInvoiceHtml(order, userInfo, discount);

                byte[] bytes = Encoding.UTF8.GetBytes(htmlContent);
                return Content(htmlContent, "text/html; charset=utf-8");
                //return File(bytes, "text/html", $"HoaDon_{orderId}.html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
