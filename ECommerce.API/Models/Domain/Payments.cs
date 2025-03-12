using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.API.Models.Domain
{
    public class Payments
    {
        [Key]
        public Guid PaymentID { get; set; } // ID thanh toán
        public Guid OrderID { get; set; }  // Liên kết đến đơn hàng
        public Guid UserID { get; set; }   // Liên kết đến người dùng
        public int PaymentMethodID { get; set; } // ID phương thức thanh toán 1: VNPAY, 2: COD
        public string PaymentStatus { get; set; } // Trạng thái (Pending, Completed, Failed)
        public string? TransactionID { get; set; } // Mã giao dịch từ cổng thanh toán
        public double AmountPaid { get; set; }    // Tổng tiền đã thanh toán
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public string? PaymentDetails { get; set; } // Thông tin chi tiết (nếu cần)

        // Navigation Properties
        [ForeignKey("PaymentMethodID")]
        public PaymentMethods PaymentMethods { get; set; }

        [ForeignKey("OrderID")]
        public Orders Orders { get; set; }
    }
}
