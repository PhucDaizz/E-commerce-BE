using ECommerce.API.Data;
using ECommerce.API.Models.Enums;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Impemention
{
    public class StalePendingConversationCleanupService : IStalePendingConversationCleanupService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StalePendingConversationCleanupService> _logger;
        private readonly TimeSpan _cleanupStalePendingThreshold = TimeSpan.FromDays(1);

        public StalePendingConversationCleanupService(AppDbContext dbContext, ILogger<StalePendingConversationCleanupService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task CleanupStalePendingConversationsAsync(CancellationToken stoppingToken)
        {
            var cutoffDatePending = DateTime.UtcNow.Subtract(_cleanupStalePendingThreshold);
            _logger.LogInformation("Starting cleanup for stale pending conversations older than {CutoffDate}", cutoffDatePending);

            int updatedCount = 0;
            var stalePendingConversations = await _dbContext.Conversations
                .Where(c => c.Status == ConversationStatus.Pending && c.StartTimeUtc < cutoffDatePending)
                .ToListAsync(stoppingToken);

            if (stalePendingConversations.Any())
            {
                foreach (var conv in stalePendingConversations)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    conv.Status = ConversationStatus.Closed;
                    conv.EndTimeUtc = DateTime.UtcNow;
                    conv.AdminUserId = null;
                }
                await _dbContext.SaveChangesAsync(stoppingToken);
                updatedCount = stalePendingConversations.Count;
            }

            if (updatedCount > 0)
            {
                _logger.LogInformation("Auto-closed {Count} stale pending conversations.", updatedCount);
            }
            else
            {
                _logger.LogInformation("No stale pending conversations found to clean up.");
            }
        }
    }
}
