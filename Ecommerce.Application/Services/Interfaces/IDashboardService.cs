using Ecommerce.Application.DTOS.Dashboard;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<RevenueResponseDTO> GetDailyRevenue();
        Task<RevenueResponseDTO> GetMonthlyRevenue();
        Task<List<CustomerLocationDTO>> GetTopCustomerLocationsAsync(int topN = 10);
        Task<LocationAnalysisResponseDTO> GetLocationAnalysisAsync(int topN = 10);

    }
}
