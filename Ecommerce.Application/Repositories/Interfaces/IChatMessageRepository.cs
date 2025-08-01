using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<IEnumerable<ChatMessage>?> GetChatHistoryAsync(Guid conversationId);
        Task<ChatMessage> AddAsync(ChatMessage chatMessage);
        Task<ChatMessage> GetLastMessagePreviewAsync(Guid conversationId);
        Task MarkMessagesAsReadAsync(Guid conversationId, string currentUserId, bool isAdmin);

    }
}
