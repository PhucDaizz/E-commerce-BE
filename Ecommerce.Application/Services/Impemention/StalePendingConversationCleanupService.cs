using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Services.Impemention
{
    public class StalePendingConversationCleanupService : IStalePendingConversationCleanupService
    {
        private readonly ILogger<StalePendingConversationCleanupService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeSpan _cleanupStalePendingThreshold = TimeSpan.FromDays(1);

        public StalePendingConversationCleanupService(ILogger<StalePendingConversationCleanupService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task CleanupStalePendingConversationsAsync(CancellationToken stoppingToken)
        {
            var cutoffDatePending = DateTime.UtcNow.Subtract(_cleanupStalePendingThreshold);
            _logger.LogInformation("Starting cleanup for stale pending conversations older than {CutoffDate}", cutoffDatePending);

            // 1. Lấy danh sách các cuộc hội thoại cần cập nhật
            var stalePendingConversations = await _unitOfWork.Conversation.GetStalePendingConversationsAsync(_cleanupStalePendingThreshold, stoppingToken);


            // 2. Cập nhật trạng thái của chúng trong bộ nhớ
            foreach (var conv in stalePendingConversations)
            {
                if (stoppingToken.IsCancellationRequested) break;
                conv.Status = ConversationStatus.Closed;
                conv.EndTimeUtc = DateTime.UtcNow;
                conv.AdminUserId = null;
            }

            if (stoppingToken.IsCancellationRequested) return;

            // 3. Lưu tất cả thay đổi trong một lần
            try
            {
                var affectedRows = await _unitOfWork.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Auto-closed {Count} stale pending conversations. {AffectedRows} changes saved.", stalePendingConversations.Count, affectedRows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save changes during stale conversation cleanup.");
            }
        }
    }
}
