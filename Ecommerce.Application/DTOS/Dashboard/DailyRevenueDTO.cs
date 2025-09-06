namespace Ecommerce.Application.DTOS.Dashboard
{
    public class DailyRevenueDTO
    {
        public DateTime Date { get; set; }          
        public double Revenue { get; set; }         
        public string Label { get; set; }           
        public string DayOfWeek { get; set; }

        public DailyRevenueDTO() {}

        public DailyRevenueDTO(DateTime date, double revenue)
        {
            Date = date;
            Revenue = revenue;
            Label = GenerateLabel(date);
            DayOfWeek = GetVietnameseDayOfWeek(date);
        }

        private string GetVietnameseDayOfWeek(DateTime date)
        {
            string[] days = { "Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7" };
            return days[(int)date.DayOfWeek];
        }

        private string GenerateLabel(DateTime date)
        {
            return date.ToString("dd/MM");
        }
    }
}
