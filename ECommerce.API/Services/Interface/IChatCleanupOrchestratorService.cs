namespace ECommerce.API.Services.Interface
{
    public interface IChatCleanupOrchestratorService
    {
        Task PerformCleanupAsync(CancellationToken stoppingToken);
    }
}
