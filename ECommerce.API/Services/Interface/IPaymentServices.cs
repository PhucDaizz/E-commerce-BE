using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Payment;
using VNPAY.NET.Models;

namespace ECommerce.API.Services.Interface
{
    public interface IPaymentServices
    {
        Task<PaymentProcessResult> processPayment(PaymentResult paymentResult,Guid userID, int PaymentMethodId, int? discountId);
        Task<PaymentProcessResult> processPaymentTWO(PaymentResult paymentResult,Guid userID, int PaymentMethodId, int? discountId);
        Task<PaymentProcessResult> processPaymentCOD(Guid userID, int? discountId, int PaymentMethodId = 2);
        Task<PaymentAmountDTO> checkAmount(Guid userId, int? discountId);
    }
}
