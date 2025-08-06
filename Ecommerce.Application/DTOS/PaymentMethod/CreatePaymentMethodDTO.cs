namespace Ecommerce.Application.DTOS.PaymentMethod
{
    public class CreatePaymentMethodDTO
    {
        public string MethodName { get; set; }
        public string? Description { get; set; }
    }
}
