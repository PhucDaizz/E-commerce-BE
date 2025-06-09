using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IChatMessageRepository
    {
        Task<IEnumerable<ChatMessage>?> GetChatHistoryAsync(Guid conversationId);
        Task<ChatMessage> AddAsync(ChatMessage chatMessage);
        Task<ChatMessage> GetLastMessagePreviewAsync(Guid conversationId);
        Task MarkMessagesAsReadAsync(Guid conversationId, string currentUserId, bool isAdmin);

    }
}
