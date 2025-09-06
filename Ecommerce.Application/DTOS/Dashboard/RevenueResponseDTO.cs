namespace Ecommerce.Application.DTOS.Dashboard
{
    public class RevenueResponseDTO
    {
        public string PeriodType { get; set; }      
        public List<object> Revenues { get; set; }  
        public double TotalRevenue { get; set; }    
        public string PeriodRange { get; set; }     

        public RevenueResponseDTO() { }

        public RevenueResponseDTO(string periodType, List<object> revenues)
        {
            PeriodType = periodType;
            Revenues = revenues;
            TotalRevenue = CalculateTotalRevenue(revenues);
            PeriodRange = GeneratePeriodRange(periodType, revenues);
        }

        private double CalculateTotalRevenue(List<object> revenues)
        {
            return revenues.Sum(item =>
            {
                if (item is DailyRevenueDTO daily)
                    return daily.Revenue;
                else if (item is MonthlyRevenueDTO monthly)
                    return monthly.Revenue;
                return 0.0;
            });
        }

        private string GeneratePeriodRange(string periodType, List<object> revenues)
        {
            if (revenues == null || !revenues.Any())
                return "";

            if (periodType == "DAILY" && revenues.First() is DailyRevenueDTO firstDaily)
            {
                var first = (DailyRevenueDTO)revenues.First();
                var last = (DailyRevenueDTO)revenues.Last();
                return $"{first.Date:dd/MM} - {last.Date:dd/MM/yyyy}";
            }

            if (periodType == "MONTHLY" && revenues.First() is MonthlyRevenueDTO firstMonthly)
            {
                var first = (MonthlyRevenueDTO)revenues.First();
                var last = (MonthlyRevenueDTO)revenues.Last();
                return $"Tháng {first.Month} - {last.Month}/{last.Year}";
            }

            return "";
        }

    }
}
