using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.Enums;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _dbContext;

        public ConversationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conversations> AddAsync(Conversations conversation)
        {
            var addedConversation = await _dbContext.Conversations.AddAsync(conversation);
            await _dbContext.SaveChangesAsync();
            return addedConversation.Entity;
        }

        public async Task<Conversations?> GetActiveConversationByClientForDisconnectAsync(string userId)
        {
            return await _dbContext.Conversations.FirstOrDefaultAsync(x => x.ClientUserId == userId);
        }

        public async Task<IEnumerable<Conversations>> GetActiveConversationsByAdminAsync(string userId)
        {
            return await _dbContext.Conversations.Where(c => c.AdminUserId == userId && c.Status == ConversationStatus.Active)
                .OrderByDescending(c => c.StartTimeUtc)
                .Include(c => c.ClientUser) 
                .ToListAsync();
        }

        public async Task<Conversations?> GetByIdAsync(Guid conversationId)
        {
            return await _dbContext.Conversations
                .Include(c => c.ClientUser)
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
        }

        public async Task<Conversations> GetConversationPedingOrActiveAsync(string userId)
        {
            return await _dbContext.Conversations
                .Include(c => c.AdminUser) 
                .Include(c => c.ChatMessages)
                .FirstOrDefaultAsync(c => c.ClientUserId == userId && (c.Status == ConversationStatus.Active || c.Status == ConversationStatus.Active));
        }

        public async Task<IEnumerable<Conversations>?> GetPendingConversationsForAdminAsync()
        {
            return await _dbContext.Conversations
                .Where(c => c.Status == ConversationStatus.Pending)
                .OrderByDescending(c => c.StartTimeUtc)
                .Include(c => c.ClientUser)
                .Include(c => c.ChatMessages)
                .ToListAsync();
        }

        public async Task<Conversations?> GetPendingOrActiveConversationByClientAsync(string userId)
        {
            return await _dbContext.Conversations.FirstOrDefaultAsync(c => c.ClientUserId == userId && 
                (c.Status == ConversationStatus.Pending || c.Status == ConversationStatus.Active));
        }

        public async Task<bool> UpdateAsync(Conversations conversations)
        {
            var updatedConversation = _dbContext.Conversations.Update(conversations);
            await _dbContext.SaveChangesAsync();
            return updatedConversation != null;
        }
    }
}
