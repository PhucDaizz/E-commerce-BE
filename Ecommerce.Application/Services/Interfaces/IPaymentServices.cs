using Ecommerce.Application.DTOS.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET.Models;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IPaymentServices
    {
        Task<PaymentProcessResult> processPayment(PaymentResult paymentResult, Guid userID, int PaymentMethodId, int? discountId);
        Task<PaymentProcessResult> processPaymentTWO(PaymentResult paymentResult, Guid userID, int PaymentMethodId, int? discountId);
        Task<PaymentProcessResult> processPaymentCOD(Guid userID, int? discountId, int PaymentMethodId = 2);
        Task<PaymentAmountDTO> checkAmount(Guid userId, int? discountId);
    }
}
