using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ExtendedIdentityUser> _userManager;

        public AuthRepository(AppDbContext dbContext, UserManager<ExtendedIdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task AddLoginAsync(string userId, string loginProvider, string providerKey)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, loginProvider));
            }
        }

        public async Task AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task<(bool IsSuccess, List<string> Errors, UserIdentityDto? NewUser)> CreateExternalUserAsync(ExternalAuthCommand command)
        {
            var newUser = new ExtendedIdentityUser
            {
                UserName = $"{command.FirstName} {command.LastName}",
                Email = command.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description).ToList(), null);
            }

            var createdUserDto = new UserIdentityDto { Id = newUser.Id, Email = newUser.Email };
            return (true, new List<string>(), createdUserDto);
        }

        public async Task<UserIdentityDto?> FindByEmailAsync(string email)
        {
            var user = await _dbContext.ExtendedIdentityUsers.FirstOrDefaultAsync(u => u.Email == email);
            return user != null ? new UserIdentityDto
            {
                Id = user.Id,
                Email = user.Email,
            } : null;
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

        public async Task<List<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<bool> HasLoginAsync(string userId, string loginProvider)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var logins = await _userManager.GetLoginsAsync(user);
            return logins.Any(l => l.LoginProvider == loginProvider);
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

        public async Task UpdateRefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(12);
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
