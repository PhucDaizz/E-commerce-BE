using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;

namespace ECommerce.API.Repositories.Impemention
{
    public class ShippingRepository : IShippingRepository
    {
        private readonly ECommerceDbContext dbContext;

        public ShippingRepository(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Shippings> CreateAsync(Shippings shipping)
        {
            await dbContext.Shippings.AddAsync(shipping);
            await dbContext.SaveChangesAsync();
            return shipping;
        }
    }
}
