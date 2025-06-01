using ECommerce.API.Data;
using ECommerce.API.Models.Enums;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Impemention
{
    public class ClosedConversationCleanupService : IClosedConversationCleanupService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ClosedConversationCleanupService> _logger;
        private readonly TimeSpan _cleanupClosedThreshold = TimeSpan.FromDays(5);

        public ClosedConversationCleanupService(AppDbContext dbContext, ILogger<ClosedConversationCleanupService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task CleanupOldClosedConversationsAsync(CancellationToken stoppingToken)
        {
            var cutoffDateClosed = DateTime.UtcNow.Subtract(_cleanupClosedThreshold);
            _logger.LogInformation("Starting cleanup for closed conversations older than {CutoffDate}", cutoffDateClosed);


            var oldClosedConversationIds = await _dbContext.Conversations
                .Where(c => c.Status == ConversationStatus.Closed && c.EndTimeUtc.HasValue && c.EndTimeUtc < cutoffDateClosed)
                .Select(c => c.ConversationId)
                .ToListAsync(stoppingToken);

            if (!oldClosedConversationIds.Any())
            {
                _logger.LogInformation("No old closed conversations found to clean up.");
                return;
            }

            _logger.LogInformation("Found {Count} old closed conversations to delete.", oldClosedConversationIds.Count);

            // Xóa các cuộc trò chuyện đã đóng cũ
            int messagesDeleted = await _dbContext.ChatMessage
                .Where(m => oldClosedConversationIds.Contains(m.ConversationId))
                .ExecuteDeleteAsync(stoppingToken);
            if (messagesDeleted > 0)
                _logger.LogInformation("Removed {Count} chat messages for old closed conversations.", messagesDeleted);


            // Xóa Conversations
            int conversationsDeleted = 0;
            var conversationsToDelete = await _dbContext.Conversations
                .Where(c => oldClosedConversationIds.Contains(c.ConversationId))
                .ToListAsync(stoppingToken);
            if (conversationsToDelete.Any())
            {
                _dbContext.Conversations.RemoveRange(conversationsToDelete);
                await _dbContext.SaveChangesAsync(stoppingToken);
                conversationsDeleted = conversationsToDelete.Count;
            }

            if (conversationsDeleted > 0)
            {
                _logger.LogInformation("Successfully removed {Count} old closed conversations.", conversationsDeleted);
            }

        }
    }
}
