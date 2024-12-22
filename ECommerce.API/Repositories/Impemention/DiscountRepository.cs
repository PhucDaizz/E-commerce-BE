using AutoMapper;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.Discount;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerce.API.Repositories.Impemention
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ECommerceDbContext dbContext;
        private readonly IMapper mapper;

        public DiscountRepository(ECommerceDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        public async Task<Discounts> CreateAsync(Discounts discounts)
        {
            await dbContext.Discounts.AddAsync(discounts);
            await dbContext.SaveChangesAsync();
            return discounts;
        }

        public async Task<Discounts?> DeleteAsync(int id)
        {
            var existing = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountID == id);
            if (existing == null)
            {
                return null;
            }
            dbContext.Discounts.Remove(existing);
            await dbContext.SaveChangesAsync(); 
            return existing;
        }

        public async Task<ListDiscountDTO?> GetAllAsync([FromQuery] int page = 1, [FromQuery] int itemsInPage = 20, [FromQuery] string sortBy = "IsActive", [FromQuery] bool isDESC = true)
        {
            var discountQuery = dbContext.Discounts.AsQueryable();
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "all":
                        discountQuery = (isDESC ? discountQuery.OrderByDescending(x => x.IsActive) : discountQuery.OrderBy(x => x.IsActive));
                        break;
                    default:
                        discountQuery = discountQuery.Where(x => x.IsActive == true);
                        break;
                }
            }
            int totalCounts = await discountQuery.CountAsync();
            int totalPages = totalCounts % itemsInPage != 0 ? totalCounts / itemsInPage + 1 : totalCounts / itemsInPage;
            var itemList = await discountQuery.Skip((page - 1) * itemsInPage).Take(itemsInPage).ToListAsync();


            return new ListDiscountDTO 
            {
                Discounts = mapper.Map<IEnumerable<DiscountDTO>>(itemList),
                TotalCount = totalCounts,
                Page = page,
                PageSize = totalPages
            }; 
        }

        public async Task<Discounts?> GetByIdAsync(int id)
        {
            var existing = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountID == id);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<Discounts?> UpdateAsync(Discounts discounts)
        {
            var existing = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountID == discounts.DiscountID);
            if (existing == null)
            {
                return null;
            }
            dbContext.Discounts.Entry(existing).CurrentValues.SetValues(discounts);
            await dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
