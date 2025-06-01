using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.User;
using ECommerce.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories.Impemention
{
    public class AuthRepository: IAuthRepository
    {
        private readonly AppDbContext dbContext;

        public AuthRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ExtendedIdentityUser> GetInforAsync(string userId)
        {
            return await dbContext.ExtendedIdentityUsers.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ListUserDTO> ListUserAsync(string? querySearch, string searchField = "Email", int page = 1, int itemInPage = 10)
        {
            var query = dbContext.ExtendedIdentityUsers.AsQueryable();
            if (!string.IsNullOrEmpty(querySearch))
            {
                if (searchField == "Email")
                {
                    query = query.Where(u => u.Email.Contains(querySearch));
                }
                else if (searchField == "FullName")
                {
                    query = query.Where(u => u.UserName.Contains(querySearch));
                }
                else if (searchField == "PhoneNumber")
                {
                    query = query.Where(u => u.PhoneNumber.Contains(querySearch));
                }
            }

            int totalItem = await query.CountAsync();
            int totalPage = (int)Math.Ceiling((double)totalItem / itemInPage);
            var users = await query
                .Skip((page - 1) * itemInPage)
                .Take(itemInPage)
                .Select(u => new InforDTO
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    Gender = u.Gender
                })
                .ToListAsync();


            return new ListUserDTO
            {
                CurrentPage = page,
                TotalPage = totalPage,
                TotalItem = totalItem,
                inforDTOs = users
            };
        }
    }
}
