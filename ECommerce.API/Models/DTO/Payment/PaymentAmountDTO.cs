namespace ECommerce.API.Models.DTO.Payment
{
    public class PaymentAmountDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public float FinalAmount { get; set; }
    }
}

