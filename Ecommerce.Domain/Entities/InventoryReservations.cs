using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Domain.Entities
{
    public class InventoryReservations
    {
        [Key]
        public int ReservationID { get; set; }
        public int ProductSizeID { get; set; }
        public Guid UserID { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime ReservationTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsExpired { get; set; } = false;
        public string? TransactionID { get; set; } 

       
        [ForeignKey("ProductSizeID")]
        public ProductSizes ProductSizes { get; set; }
    }
}
