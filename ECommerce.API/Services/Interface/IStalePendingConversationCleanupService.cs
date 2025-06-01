namespace ECommerce.API.Services.Interface
{
    public interface IStalePendingConversationCleanupService
    {
        Task CleanupStalePendingConversationsAsync(CancellationToken stoppingToken);
    }
}
