using AutoMapper;
using Ecommerce.Application.DTOS.Discount;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public DiscountRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Discounts?> ActiveAsync(int discountId)
        {
            var existing = await _dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountID == discountId);
            if (existing == null)
            {
                return null;
            }
            existing.IsActive = !existing.IsActive;
            // cach 2
            _dbContext.Discounts.Update(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<Discounts> CreateAsync(Discounts discounts)
        {
            await _dbContext.Discounts.AddAsync(discounts);
            await _dbContext.SaveChangesAsync();
            return discounts;
        }

        public async Task<bool> DecrementDiscountQuantityAsync(int discountId)
        {
            var discount = await _dbContext.Discounts.FindAsync(discountId);
            if (discount == null || discount.Quantity <= 0)
            {
                return false;
            }

            discount.Quantity -= 1;

            if (discount.Quantity == 0)
            {
                discount.IsActive = false;
            }
            return true;
        }

        public async Task<Discounts?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Discounts.Include(x => x.Orders).FirstOrDefaultAsync(x => x.DiscountID == id);
            if (existing == null)
            {
                return null;
            }

            if (existing.Orders.Any())  // existing.Orders.Count > 0
            {
                return null;
            }
            _dbContext.Discounts.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<ListDiscountDTO?> GetAllAsync(int page = 1, int itemsInPage = 20, string sortBy = "all", bool isDESC = true)
        {
            var discountQuery = _dbContext.Discounts.AsQueryable();
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
                Discounts = _mapper.Map<IEnumerable<DiscountDTO>>(itemList),
                TotalCount = totalCounts,
                Page = page,
                PageSize = totalPages
            };
        }

        public async Task<Discounts?> GetByIdAsync(int id)
        {
            var existing = await _dbContext.Discounts.Include(x => x.Orders).FirstOrDefaultAsync(x => x.DiscountID == id);
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
            var existing = await _dbContext.Discounts.FirstOrDefaultAsync(x => x.Code == code);
            if (existing != null && DateTime.Now < existing.EndDate && existing.IsActive)
            {
                return existing;
            }
            return null;
        }

        public async Task<int> GetUserUsageCountAsync(Guid userId, int discountId)
        {
            return await _dbContext.Orders
                               .Where(o => o.UserID == userId && o.DiscountID == discountId)
                               .CountAsync();
        }

        public async Task<Discounts?> UpdateAsync(Discounts discounts)
        {
            var existing = await _dbContext.Discounts.Include(x => x.Orders).FirstOrDefaultAsync(x => x.DiscountID == discounts.DiscountID);
            if (existing == null)
            {
                return null;
            }
            if (!existing.Orders.Any()) // if voucher is not used
            {
                _dbContext.Discounts.Entry(existing).CurrentValues.SetValues(discounts);
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
                _dbContext.Discounts.Entry(existing).CurrentValues.SetValues(discountNew);
            }

            await _dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
