namespace Ecommerce.Application.DTOS.Dashboard
{
    public class LocationAnalysisResponseDTO
    {
        public List<CustomerLocationDTO> Locations { get; set; }
        public int TotalLocations { get; set; }
        public int TotalOrders { get; set; }
        public double TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }

        public LocationAnalysisResponseDTO()
        {
            Locations = new List<CustomerLocationDTO>();
        }
    }
}
