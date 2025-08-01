using Ecommerce.Application.DTOS.Conversation;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IConversationRepository
    {
        Task<ActiveConversationInfoDto> GetConversationPedingOrActiveAsync(string userId);
        Task<IEnumerable<PendingConversationInfo>> GetPendingConversationsForAdminAsync();
        Task<Conversations?> GetActiveConversationByClientForDisconnectAsync(string userId);
        Task<Conversations?> GetPendingOrActiveConversationByClientAsync(string userId);
        Task<Conversations> AddAsync(Conversations conversation);
        Task<bool> UpdateAsync(Conversations conversations);
        Task<bool> AcceptChatAsync(Guid conversationId, string adminUserId);
        Task<bool> UpdateLastActivityTimeAsync(Guid conversationId);
        Task<bool> CloseChatAsync(Guid conversationId);
        Task<ConversationDetailsDto?> GetByIdAsync(Guid conversationId);
        Task<IEnumerable<ConversationDetailsDto>> GetActiveConversationsByAdminAsync(string userId);

    }
}
