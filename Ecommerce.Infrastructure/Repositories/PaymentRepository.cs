using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;

        public PaymentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Payments> CreateAsync(Payments payment)
        {
            await _dbContext.Payments.AddAsync(payment);
            return payment;
        }

        public async Task<bool> ExistsByTransactionIdAsync(string transactionId)
        {
            return await _dbContext.Payments.AnyAsync(p => p.TransactionID == transactionId);
        }
    }
}
