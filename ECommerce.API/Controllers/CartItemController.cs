using AutoMapper;
using Ecommerce.Application.DTOS.CartItem;
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
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;
        private readonly ICartItemServices _cartItemServices;

        public CartItemController(ICartItemRepository cartItemRepository, IMapper mapper, ICartItemServices cartItemServices)
        {
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
            _cartItemServices = cartItemServices;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("AddToCart")]
        public async Task<IActionResult> Create([FromBody] CreateCartItemDTO cartItemDTO)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("Please login again!.");
            }

            var cartItem = _mapper.Map<CartItems>(cartItemDTO);

            cartItem.CreatedAt = DateTime.Now;
            cartItem.UpdatedAt = DateTime.Now;
            cartItem.UserID = Guid.Parse(userIdClaim.Value);

            var itemAdd = await _cartItemServices.AddAsync(cartItem);

            if (itemAdd == null)
            {
                return BadRequest("Quantity must be more than 0.");
            }

            var result = _mapper.Map<CartItemDTO>(itemAdd);

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "User")]
        [Route("UpdateCartItem")]
        public async Task<IActionResult> Update(EditCartItemDTO editCartItemDTO)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Please login again.");
            }

            var cartItem = _mapper.Map<CartItems>(editCartItemDTO);
            cartItem.UserID = Guid.Parse(userIdClaim.Value);
            cartItem.UpdatedAt = DateTime.Now;

            var itemUpdate = await _cartItemServices.UpdateAsync(cartItem);
            if (itemUpdate == null)
            {
                return Ok("Cart item not found or quantity must be more than 0.");
            }

            var result = _mapper.Map<CartItemDTO>(itemUpdate);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Please login again.");
            }
            var userId = Guid.Parse(userIdClaim.Value);

            var cartList = await _cartItemRepository.GetAllAsync(userId);
            if (!cartList.Any())
            {
                return Ok("Your cart is empty");
            }
            var result = _mapper.Map<IEnumerable<CartItemListDTO>>(cartList);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "User")]
        [Route("EmptyCart")]
        public async Task<IActionResult> EmptyCart()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return  Unauthorized("Please login again.");
            }
            var userId = Guid.Parse(userIdClaim.Value);

            var result = await _cartItemRepository.DeleteAllByUserIDAsync(userId);
            if(result == false)
            {
                return NotFound("Your cart is empty or UserId is not match");
            }
            return Ok("All items have been removed from your cart.");
        }
        
    }
}