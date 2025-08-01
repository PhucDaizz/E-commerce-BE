using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
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
