using ECommerce.API.Models.Domain;

namespace ECommerce.API.Repositories.Interface
{
    public interface IPaymentRepository
    {
        Task<Payments> CreateAsync(Payments payment);
    }
}
