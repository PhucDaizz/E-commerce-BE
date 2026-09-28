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
        Task<PrepareBankingPaymentResult> PrepareBankingPaymentAsync(Guid userId, string? note, int? discountId, string ipAddress);
        Task<PaymentProcessResult> ConfirmBankingPaymentAsync(PaymentResult paymentResult);
        Task HandleFailedBankingPaymentAsync(string txnRef);
        Task<BankingPaymentStatusDTO?> GetBankingPaymentStatusAsync(string txnRef, Guid callerUserId);
        Task<PaymentProcessResult> processPaymentCOD(Guid userID, int? discountId, int PaymentMethodId = 2);
    }
}
