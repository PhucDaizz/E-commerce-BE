
using ECommerce.API.Services.Interface;

namespace ECommerce.API.BackgroundServices
{
    public class ChatCleanupService : BackgroundService
    {
        private readonly ILogger<ChatCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _jobInterval = TimeSpan.FromHours(24);

        public ChatCleanupService(ILogger<ChatCleanupService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
           
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Chat Cleanup Background Service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Chat Cleanup Background Service is triggering work at: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var orchestrator = scope.ServiceProvider.GetRequiredService<IChatCleanupOrchestratorService>();
                        await orchestrator.PerformCleanupAsync(stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Chat Cleanup Background Service execution was canceled during orchestration.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unhandled error occurred in Chat Cleanup Background Service execution.");
                }

                try
                {
                    _logger.LogInformation("Chat Cleanup Background Service finished work. Next run in {JobInterval}", _jobInterval);
                    await Task.Delay(_jobInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Chat Cleanup Background Service delay was canceled.");
                }
            }
            _logger.LogInformation("Chat Cleanup Background Service is stopping.");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Chat Cleanup Background Service is performing graceful shutdown.");
            await base.StopAsync(stoppingToken);
            _logger.LogInformation("Chat Cleanup Background Service has stopped.");
        }
    }
}
