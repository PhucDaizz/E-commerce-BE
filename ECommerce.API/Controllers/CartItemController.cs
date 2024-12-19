using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.CartItem;
using ECommerce.API.Repositories.Impemention;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ECommerceDbContext dbContext;
        private readonly ICartItemRepository cartItemRepository;
        private readonly IMapper mapper;
        private readonly ICartItemServices cartItemServices;

        public CartItemController(ECommerceDbContext dbContext, ICartItemRepository cartItemRepository, IMapper mapper, ICartItemServices cartItemServices)
        {
            this.dbContext = dbContext;
            this.cartItemRepository = cartItemRepository;
            this.mapper = mapper;
            this.cartItemServices = cartItemServices;
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

            var cartItem = mapper.Map<CartItems>(cartItemDTO);

            cartItem.CreatedAt = DateTime.Now;
            cartItem.UpdatedAt = DateTime.Now;
            cartItem.UserID = Guid.Parse(userIdClaim.Value);

            var itemAdd = await cartItemServices.AddAsync(cartItem);

            if (itemAdd == null)
            {
                return BadRequest("Quantity must be more than 0.");
            }

            var result = mapper.Map<CartItemDTO>(itemAdd);

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

            var cartItem = mapper.Map<CartItems>(editCartItemDTO);
            cartItem.UserID = Guid.Parse(userIdClaim.Value);
            cartItem.UpdatedAt = DateTime.Now;

            var itemUpdate = await cartItemServices.UpdateAsync(cartItem);
            if (itemUpdate == null)
            {
                return BadRequest("Cart item not found or quantity must be more than 0.");
            }

            var result = mapper.Map<CartItemDTO>(itemUpdate);

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

            var cartList = await cartItemRepository.GetAllAsync(userId);
            if (!cartList.Any())
            {
                return Ok("Your cart is empty");
            }
            var result = mapper.Map<IEnumerable<CartItemDTO>>(cartList);
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

            var result = await cartItemRepository.DeleteAllByUserIDAsync(userId);
            if(result == false)
            {
                return NotFound("Your cart is empty or UserId is not match");
            }
            return Ok("All items have been removed from your cart.");
        }
        
    }
}