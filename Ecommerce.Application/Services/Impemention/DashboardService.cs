using Ecommerce.Application.DTOS.Dashboard;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;

namespace Ecommerce.Application.Services.Impemention
{
    public class DashboardService: IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<RevenueResponseDTO> GetDailyRevenue()
        {
            DateTime endDate = DateTime.Now.Date;
            DateTime startDate = endDate.AddDays(-6); 

            var dailyRevenues = await _dashboardRepository.FindDailyRevenue(startDate, endDate);

            var result = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = startDate.AddDays(i);
                var found = dailyRevenues.FirstOrDefault(d => d.Date.Date == currentDate.Date);

                result.Add(found ?? new DailyRevenueDTO(currentDate, 0.0));
            }

            return new RevenueResponseDTO("DAILY", result);
        }

        public async Task<LocationAnalysisResponseDTO> GetLocationAnalysisAsync(int topN = 10)
        {
            var locations = await _dashboardRepository.GetTopCustomerLocationsAsync(topN);

            return new LocationAnalysisResponseDTO
            {
                Locations = locations,
                TotalLocations = locations.Count,
                TotalOrders = locations.Sum(x => x.OrderCount),
                TotalRevenue = locations.Sum(x => x.TotalRevenue),
                TotalCustomers = locations.Sum(x => x.CustomerCount)
            };
        }

        public async Task<RevenueResponseDTO> GetMonthlyRevenue()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startDate = currentDate.AddMonths(-11); 

            var monthlyRevenues = await _dashboardRepository.FindMonthlyRevenue(
                startDate.Year, startDate.Month,
                currentDate.Year, currentDate.Month);

            var result = new List<object>();
            for (int i = 0; i < 12; i++)
            {
                DateTime current = startDate.AddMonths(i);
                var found = monthlyRevenues.FirstOrDefault(m =>
                    m.Year == current.Year && m.Month == current.Month);

                result.Add(found ?? new MonthlyRevenueDTO(current.Year, current.Month, 0.0));
            }

            return new RevenueResponseDTO("MONTHLY", result);
        }

        public async Task<List<CustomerLocationDTO>> GetTopCustomerLocationsAsync(int topN = 10)
        {
            return await _dashboardRepository.GetTopCustomerLocationsAsync(topN);
        }
    }
}
