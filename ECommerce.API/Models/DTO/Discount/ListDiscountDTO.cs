namespace ECommerce.API.Models.DTO.Discount
{
    public class ListDiscountDTO
    {
        public IEnumerable<DiscountDTO> Discounts { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
