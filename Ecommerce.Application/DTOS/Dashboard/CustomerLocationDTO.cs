namespace Ecommerce.Application.DTOS.Dashboard
{
    public class CustomerLocationDTO
    {
        public string Province { get; set; }
        public int OrderCount { get; set; }
        public double TotalRevenue { get; set; }
        public int CustomerCount { get; set; }
        public string Label { get; set; }

        public CustomerLocationDTO() { }

        public CustomerLocationDTO(string province, int orderCount, double totalRevenue, int customerCount)
        {
            Province = province;
            OrderCount = orderCount;
            TotalRevenue = totalRevenue;
            CustomerCount = customerCount;
            Label = province;
        }
    }
}
