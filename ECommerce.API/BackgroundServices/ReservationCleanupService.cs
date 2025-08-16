
using Ecommerce.Application.Services.Interfaces;

namespace ECommerce.API.BackgroundServices
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly ILogger<ReservationCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ReservationCleanupService(ILogger<ReservationCleanupService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reservationService = scope.ServiceProvider.GetRequiredService<IInventoryReservationService>();

                        _logger.LogInformation("Running cleanup for expired reservations at {time}", DateTimeOffset.Now);
                        var deletedCount = await reservationService.DeleteExpiredReservationsAsync();

                        if (deletedCount > 0)
                        {
                            _logger.LogInformation("Deleted {Count} expired reservations at {Time}", deletedCount, DateTime.Now);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up expired reservations.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("Reservation Cleanup Service is stopping.");
        }
    }
}
