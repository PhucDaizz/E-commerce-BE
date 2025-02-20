namespace ECommerce.API.Models.DTO.Payment
{
    public class PaymentDTO
    {
        public Guid PaymentID { get; set; } // ID thanh toán
        public int PaymentMethodID { get; set; } // ID phương thức thanh toán
        public string PaymentStatus { get; set; } // Trạng thái (Pending, Completed, Failed)
        public string? TransactionID { get; set; } // Mã giao dịch từ cổng thanh toán
        public double AmountPaid { get; set; }    // Tổng tiền đã thanh toán
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public string? PaymentDetails { get; set; } // Thông tin chi tiết (nếu cần)
    }
}
