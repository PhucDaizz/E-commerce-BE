using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethods> AddAsync(PaymentMethods paymentMethods);

        Task<IEnumerable<PaymentMethods>?> GetAllAsync();   
    }
}
