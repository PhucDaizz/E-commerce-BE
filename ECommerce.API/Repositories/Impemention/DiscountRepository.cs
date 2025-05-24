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
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;

        public DiscountRepository(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<Discounts?> ActiveAsync(int discountId)
        {
            var existing = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountID == discountId);
            if(existing == null)
            {
                return null;
            }
            existing.IsActive = !existing.IsActive;
            // cach 2
            dbContext.Discounts.Update(existing);
            await dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<Discounts> CreateAsync(Discounts discounts)
        {
            await dbContext.Discounts.AddAsync(discounts);
            await dbContext.SaveChangesAsync();
            return discounts;
        }

        public async Task<Discounts?> DeleteAsync(int id)
        {
            var existing = await dbContext.Discounts.Include(x => x.Orders).FirstOrDefaultAsync(x => x.DiscountID == id);
            if (existing == null)
            {
                return null;
            }
            
            if (existing.Orders.Any())  // existing.Orders.Count > 0
            {
                return null;
            }
            dbContext.Discounts.Remove(existing);
            await dbContext.SaveChangesAsync(); 
            return existing;
        }

        public async Task<ListDiscountDTO?> GetAllAsync([FromQuery] int page = 1, [FromQuery] int itemsInPage = 20, [FromQuery] string sortBy = "all", [FromQuery] bool isDESC = true)
        {
            var discountQuery = dbContext.Discounts.AsQueryable();
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "active":
                        discountQuery = (isDESC ? discountQuery.OrderByDescending(x => x.EndDate).Where(x => x.IsActive == true) : discountQuery.OrderBy(x => x.EndDate).Where(x => x.IsActive == true));
                        break;
                    case "inactive":
                        discountQuery = (isDESC ? discountQuery.OrderByDescending(x => x.EndDate).Where(x => x.IsActive == false) : discountQuery.OrderBy(x => x.EndDate).Where(x => x.IsActive == false));
                        break;
                    default:
                        discountQuery = (isDESC ? discountQuery.OrderByDescending(x => x.EndDate) : discountQuery.OrderBy(x => x.EndDate));
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
            var existing = await dbContext.Discounts.Include(x => x.Orders).FirstOrDefaultAsync(x => x.DiscountID == id);
            if (existing == null)
            {
                return null;
            }
            if (existing.Orders.Any())
            {
                existing.Description = string.IsNullOrEmpty(existing.Description)
                    ? "[USED]"
                    : $"{existing.Description} [USED]";
            }
            return existing;
        }

        public async Task<Discounts?> GetDiscountByCodeAsync(string code)
        {
            var existing = await dbContext.Discounts.FirstOrDefaultAsync(x => x.Code == code);
            if (existing != null && DateTime.Now < existing.EndDate && existing.IsActive)
            {
                return existing;
            }
            return null;
        }

        public async Task<Discounts?> UpdateAsync(Discounts discounts)
        {
            var existing = await dbContext.Discounts.Include(x => x.Orders).FirstOrDefaultAsync(x => x.DiscountID == discounts.DiscountID);
            if (existing == null)
            {
                return null;
            }
            if(!existing.Orders.Any()) // if voucher is not used
            {
                dbContext.Discounts.Entry(existing).CurrentValues.SetValues(discounts);
            }
            else
            {
                var endDate = discounts.EndDate < DateTime.UtcNow ? existing.EndDate : discounts.EndDate;
                var discountNew = new Discounts
                {
                    Code = discounts.Code,
                    Quantity = discounts.Quantity,
                    Description = discounts.Description,
                    MaxUsagePerUser = discounts.MaxUsagePerUser,
                    IsActive = discounts.IsActive,
                    DiscountID = existing.DiscountID,
                    DiscountType = existing.DiscountType,
                    DiscountValue = existing.DiscountValue,
                    MinOrderValue = existing.MinOrderValue,
                    StartDate = existing.StartDate,
                    EndDate = endDate
                };
                dbContext.Discounts.Entry(existing).CurrentValues.SetValues(discountNew);
            }

            await dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
