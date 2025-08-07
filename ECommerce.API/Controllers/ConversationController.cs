using Ecommerce.Application.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IChatMessageRepository _chatMessageRepository;

        public ConversationController(IChatMessageRepository chatMessageRepository)
        {
            _chatMessageRepository = chatMessageRepository;
        }

        [Authorize]
        [HttpPost("{conversationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(Guid conversationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            await _chatMessageRepository.MarkMessagesAsReadAsync(conversationId, userId, isAdmin);

            return Ok(new { Message = "Messages marked as read successfully." });
        }

    }
}
