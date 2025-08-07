using Ecommerce.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class ChatCleanupOrchestratorService : IChatCleanupOrchestratorService
    {
        private readonly IClosedConversationCleanupService _closedConversationCleanupService;
        private readonly IStalePendingConversationCleanupService _stalePendingConversationCleanupService;
        private readonly ILogger<ChatCleanupOrchestratorService> _logger;

        public ChatCleanupOrchestratorService(IClosedConversationCleanupService closedConversationCleanupService, IStalePendingConversationCleanupService stalePendingConversationCleanupService, ILogger<ChatCleanupOrchestratorService> logger)
        {
            _closedConversationCleanupService = closedConversationCleanupService;
            _stalePendingConversationCleanupService = stalePendingConversationCleanupService;
            _logger = logger;
        }
        public async Task PerformCleanupAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Chat cleanup orchestration started.");
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Cleanup orchestration canceled before starting tasks.");
                return;
            }

            // Thực hiện dọn dẹp closed conversations
            _logger.LogInformation("Starting closed conversation cleanup task...");
            await _closedConversationCleanupService.CleanupOldClosedConversationsAsync(stoppingToken);
            _logger.LogInformation("Closed conversation cleanup task finished.");

            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Cleanup orchestration canceled after closed conversation cleanup.");
                return;
            }

            // Thực hiện dọn dẹp stale pending conversations
            _logger.LogInformation("Starting stale pending conversation cleanup task...");
            await _stalePendingConversationCleanupService.CleanupStalePendingConversationsAsync(stoppingToken);
            _logger.LogInformation("Stale pending conversation cleanup task finished.");

            _logger.LogInformation("Chat cleanup orchestration completed.");

        }
    }
}
