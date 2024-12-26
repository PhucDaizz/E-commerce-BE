using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Payment;
using VNPAY.NET.Models;

namespace ECommerce.API.Services.Interface
{
    public interface IPaymentServices
    {
        Task<PaymentProcessResult> processPayment(PaymentResult paymentResult,Guid userID, int PaymentMethodId, int? discountId);
    }
}
