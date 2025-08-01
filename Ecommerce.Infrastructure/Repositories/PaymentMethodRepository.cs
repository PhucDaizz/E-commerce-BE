using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class PaymentMethodRepository: IPaymentMethodRepository
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
