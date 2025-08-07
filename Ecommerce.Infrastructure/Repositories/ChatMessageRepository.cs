using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ChatMessageRepository> _logger;

        public ChatMessageRepository(AppDbContext dbContext, ILogger<ChatMessageRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ChatMessage> AddAsync(ChatMessage chatMessage)
        {
            try
            {
                var entity = await _dbContext.ChatMessage.AddAsync(chatMessage);
                await _dbContext.SaveChangesAsync();
                return entity.Entity;

            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DbUpdateException while adding ChatMessage. Message: {Message}. InnerException: {InnerExMessage}",
                         dbEx.Message, dbEx.InnerException?.Message);
                // Có thể xem xét dbEx.Entries để biết entity nào gây lỗi
                foreach (var entry in dbEx.Entries)
                {
                    _logger.LogError("Entity of type {EntityType} in state {EntityState} could not be saved.",
                                     entry.Entity.GetType().Name, entry.State);
                }
                throw;
            }
        }

        public async Task DeleteMessagesByConversationIdsAsync(List<Guid> conversationIds)
        {
            var messagesToDelete = await _dbContext.ChatMessage
                .Where(m => conversationIds.Contains(m.ConversationId))
                .ToListAsync();

            if (messagesToDelete.Any())
            {
                _dbContext.ChatMessage.RemoveRange(messagesToDelete);
            }
        }

        public async Task<IEnumerable<ChatMessage>?> GetChatHistoryAsync(Guid conversationId)
        {
            return await _dbContext.ChatMessage.Where(c => c.ConversationId == conversationId)
                .OrderBy(c => c.SentTimeUtc)
                .Take(100)
                .ToListAsync();
        }

        public async Task<ChatMessage> GetLastMessagePreviewAsync(Guid conversationId)
        {
            return await _dbContext.ChatMessage
                 .Where(c => c.ConversationId == conversationId)
                 .OrderByDescending(c => c.SentTimeUtc)
                 .FirstOrDefaultAsync();
        }

        public async Task MarkMessagesAsReadAsync(Guid conversationId, string currentUserId, bool isAdmin)
        {
            if (isAdmin)
            {
                await _dbContext.ChatMessage
                    .Where(m => m.ConversationId == conversationId
                             && !m.IsReadByAdmin)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(m => m.IsReadByAdmin, true));
            }
            else
            {
                await _dbContext.ChatMessage
                    .Where(m => m.ConversationId == conversationId
                             && !m.IsReadByClient)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(m => m.IsReadByClient, true));
            }
        }

    }
}
