using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IConversationRepository
    {
        Task<Conversations> GetConversationPedingOrActiveAsync(string userId);
        Task<IEnumerable<Conversations>> GetPendingConversationsForAdminAsync();
        Task<Conversations?> GetActiveConversationByClientForDisconnectAsync(string userId);
        Task<Conversations?> GetPendingOrActiveConversationByClientAsync(string userId);
        Task<Conversations> AddAsync(Conversations conversation);
        Task<bool> UpdateAsync(Conversations conversations);
        Task<Conversations?> GetByIdAsync(Guid conversationId);
        Task<IEnumerable<Conversations>> GetActiveConversationsByAdminAsync(string userId);

    }
}
