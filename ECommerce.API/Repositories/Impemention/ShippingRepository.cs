using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Shipping;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class ShippingRepository : IShippingRepository
    {
        private readonly AppDbContext dbContext;

        public ShippingRepository(AppDbContext dbContext, IOrderRepository orderRepository)
        {
            this.dbContext = dbContext;
        }
        public async Task<Shippings> CreateAsync(Shippings shipping)
        {
            await dbContext.Shippings.AddAsync(shipping);
            await dbContext.SaveChangesAsync();
            return shipping;
        }

        public async Task<Shippings?> UpdateAsync(Guid orderId, UpdateShippingDTO shipping)
        {
            var existingShipping = await dbContext.Shippings.FirstOrDefaultAsync(x => x.OrderID == orderId);
            if (existingShipping == null)
            {
                return null;
            }
            existingShipping.ShippingServicesID = shipping.ShippingServicesID ?? existingShipping.ShippingServicesID;
            existingShipping.ShippingFee = shipping.ShippingFee ?? existingShipping.ShippingFee;
            existingShipping.ShippingStatus = shipping.ShippingStatus ?? existingShipping.ShippingStatus;
            /*existingShipping.EstimatedDeliveryDate = shipping.EstimatedDeliveryDate ?? existingShipping.EstimatedDeliveryDate;*/
            existingShipping.ActualDeliveryDate = shipping.ActualDeliveryDate ?? existingShipping.ActualDeliveryDate;
            existingShipping.UpdatedAt = DateTime.Now;

            dbContext.Entry(existingShipping).CurrentValues.SetValues(existingShipping);
            await dbContext.SaveChangesAsync();
            return existingShipping;

        }
    }
}
