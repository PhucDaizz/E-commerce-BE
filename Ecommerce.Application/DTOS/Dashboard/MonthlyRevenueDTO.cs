namespace Ecommerce.Application.DTOS.Dashboard
{
    public class MonthlyRevenueDTO
    {
        public int Year { get; set; }               
        public int Month { get; set; }              
        public double Revenue { get; set; }         
        public string Label { get; set; }

        public MonthlyRevenueDTO() { }

        public MonthlyRevenueDTO(int year, int month, double revenue)
        {
            Year = year;
            Month = month;
            Revenue = revenue;
            Label = GenerateLabel(month);
        }

        private string GenerateLabel(int month)
        {
            return $"Tháng {month}";
        }

        public DateTime GetDate()
        {
            return new DateTime(Year, Month, 1);
        }
    }
}
