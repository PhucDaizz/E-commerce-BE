namespace ECommerce.API.Models.DTO.PaymentMethod
{
    public class PaymentMethodDTO
    {
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; }
        public string? Description { get; set; }
    }
}
