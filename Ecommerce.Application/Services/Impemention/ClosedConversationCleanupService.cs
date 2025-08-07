using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class ClosedConversationCleanupService : IClosedConversationCleanupService
    {
        private readonly ILogger<ClosedConversationCleanupService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeSpan _cleanupClosedThreshold = TimeSpan.FromDays(5);

        public ClosedConversationCleanupService(ILogger<ClosedConversationCleanupService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task CleanupOldClosedConversationsAsync(CancellationToken stoppingToken)
        {
            var cutoffDateClosed = DateTime.UtcNow.Subtract(_cleanupClosedThreshold);
            _logger.LogInformation("Starting cleanup for closed conversations older than {CutoffDate}", cutoffDateClosed);

            // 1. Lấy danh sách các cuộc hội thoại cần xóa
            var oldClosedConversationIds = await _unitOfWork.Conversation.GetOldClosedConversationsForCleanupAsync(_cleanupClosedThreshold, stoppingToken);

            if (!oldClosedConversationIds.Any())
            {
                _logger.LogInformation("No old closed conversations found to clean up.");
                return;
            }


            var conversationIdsToDelete = oldClosedConversationIds.Select(c => c.ConversationId).ToList();
            _logger.LogInformation("Found {Count} old closed conversations to delete.", conversationIdsToDelete.Count);

            // 2. Xóa các tin nhắn liên quan 
            await _unitOfWork.ChatMessage.DeleteMessagesByConversationIdsAsync(conversationIdsToDelete);

            // 3. Xóa các cuộc hội thoại 
            _unitOfWork.Conversation.DeleteRange(oldClosedConversationIds);


            try
            {
                var affectedRows = await _unitOfWork.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Cleanup successful. {Count} changes saved to database.", affectedRows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save changes during cleanup.");
            }
        }
    }
}
