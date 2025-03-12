using ECommerce.API.Models.DTO.Product;

namespace ECommerce.API.Models.DTO.OrderDetail
{
    public class GetOrderDetailDTO
    {
        public int OrderDetailID { get; set; }
        public Guid OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public ProductImageCartDTO ProductDTO { get; set; }
    }
}
