using Ecommerce.Application.DTOS.Inventory;

namespace Ecommerce.Application.DTOS.Payment
{
    public class PrepareBankingPaymentRequest
    {
        public string? Note { get; set; }
        public int? DiscountId { get; set; }
    }

    public class PrepareBankingPaymentResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? OrderId { get; set; }
        public string? TransactionRef { get; set; }
        public string? PaymentUrl { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public double TotalAmount { get; set; }
        public CartValidationResultDTO? Validation { get; set; }
    }

    public class BankingPaymentStatusDTO
    {
        public Guid OrderId { get; set; }
        public int Status { get; set; }
        public double TotalAmount { get; set; }
        public string TransactionRef { get; set; } = string.Empty;
    }
}
