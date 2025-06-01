namespace ECommerce.API.Services.Interface
{
    public interface IClosedConversationCleanupService
    {
        Task CleanupOldClosedConversationsAsync(CancellationToken stoppingToken);
    }
}
