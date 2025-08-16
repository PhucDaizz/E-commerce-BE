using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryReservationController : ControllerBase
    {
        private readonly IInventoryReservationService _inventoryReservationService;
        private readonly ICartItemRepository _cartItemRepository;

        public InventoryReservationController(IInventoryReservationService inventoryReservationService, ICartItemRepository cartItemRepository)
        {
            _inventoryReservationService = inventoryReservationService;
            _cartItemRepository = cartItemRepository;
        }

        [HttpPost("ReserveInventory")]
        [Authorize]
        public async Task<IActionResult> ReserveInventory()
        {
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Please login again!.");
                }
                var userId = Guid.Parse(userIdClaim.Value);

                // Get cart items
                var cartItems = await _cartItemRepository.GetAllAsync(userId);
                if (cartItems == null || !cartItems.Any())
                    return BadRequest("Cart is empty");

                // Reserve inventory
                var reservationSuccess = await _inventoryReservationService.CreateReservationForCheckoutAsync(userId, cartItems);

                if (reservationSuccess)
                {
                    return Ok(new
                    {
                        Message = "Inventory reserved successfully",
                        ReservationExpiry = DateTime.Now.AddMinutes(15)
                    });
                }

                return BadRequest("Unable to reserve inventory. Some items may be out of stock.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("CheckReservationStatus")]
        [Authorize]
        public async Task<IActionResult> CheckReservationStatus()
        {
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Please login again!.");
                }
                var userId = Guid.Parse(userIdClaim.Value);

                // Get cart items
                var cartItems = await _cartItemRepository.GetAllAsync(userId);
                if (cartItems == null || !cartItems.Any())
                    return BadRequest("Cart is empty");

                var isAvailable = await _inventoryReservationService.IsInventoryAvailableAsync(cartItems);

                return Ok(new { IsInventoryAvailable = isAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("ReleaseReservation")]
        [Authorize]
        public async Task<IActionResult> ReleaseReservation()
        {
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Please login again!.");
                }
                var userId = Guid.Parse(userIdClaim.Value);

                var released = await _inventoryReservationService.ReleaseReservationAsync(userId);

                if (released)
                {
                    return Ok("Reservation released successfully");
                }

                return Ok("No active reservations found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
