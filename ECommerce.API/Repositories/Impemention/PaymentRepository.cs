using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;

namespace ECommerce.API.Repositories.Impemention
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext dbContext;

        public PaymentRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Payments> CreateAsync(Payments payment)
        {
            await dbContext.Payments.AddAsync(payment);
            await dbContext.SaveChangesAsync();
            return payment;
        }
    }
}
