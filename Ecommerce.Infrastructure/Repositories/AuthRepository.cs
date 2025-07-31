using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _dbContext;

        public AuthRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InforDTO?> GetInforAsync(string userId)
        {
            var user = await _dbContext.ExtendedIdentityUsers.FirstOrDefaultAsync(u => u.Id == userId);
            return user != null ? new InforDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            }: null;
        }

        public async Task<ListUserDTO> ListUserAsync(string? querySearch, string searchField = "Email", int page = 1, int itemInPage = 10)
        {
            var query = _dbContext.ExtendedIdentityUsers.AsQueryable();
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
