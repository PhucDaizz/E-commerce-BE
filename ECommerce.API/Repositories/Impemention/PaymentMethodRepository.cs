using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly AppDbContext dbContext;

        public PaymentMethodRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<PaymentMethods> AddAsync(PaymentMethods paymentMethods)
        {
            await dbContext.PaymentMethods.AddAsync(paymentMethods);
            await dbContext.SaveChangesAsync();
            return paymentMethods;
        }

        public async Task<IEnumerable<PaymentMethods>?> GetAllAsync()
        {
            return await dbContext.PaymentMethods.ToListAsync();
        }
    }
}
