using Ecommerce.Application.Common.Utils;
using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

        public async Task<(bool IsAuthenticated, UserIdentityDto? User)> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (false, null);
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordCorrect)
            {
                return (false, null);
            }

            var userDto = new UserIdentityDto
            {
                Id = user.Id,
                Email = user.Email
            };

            return (true, userDto);
        }

        public async Task<(bool IsSuccess, List<string> Errors)> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return (false, new List<string> { "User Id and Token are required." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, new List<string> { "User not found." });
            }

            // Giải mã token từ định dạng an toàn cho URL
            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    return (true, new List<string>());
                }
                else
                {
                    return (false, result.Errors.Select(e => e.Description).ToList());
                }
            }
            catch (FormatException)
            {
                return (false, new List<string> { "Invalid token format." });
            }
        }

        public async Task<(bool IsSuccess, List<string> Errors, UserIdentityDto? NewUser)> CreateExternalUserAsync(ExternalAuthCommand command)
        {
            var userName = StringUtils.Slugify($"{command.FirstName} {command.LastName}");
            var newUser = new ExtendedIdentityUser
            {
                UserName = userName,
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

        public async Task<(bool IsSuccess, List<string> Errors, UserIdentityDto? NewUser)> CreateUserAsync(RegisterRequestDto command)
        {
            var user = new ExtendedIdentityUser
            {
                UserName = command.UserName?.Trim(),
                Email = command.Email?.Trim(),
                PhoneNumber = command.PhoneNumber?.Trim(),
            };

            var result = await _userManager.CreateAsync(user, command.Password);

            if (result.Succeeded)
            {
                var userDto = new UserIdentityDto
                {
                    Id = user.Id,
                    Email = user.Email
                };
                return (true, new List<string>(), userDto);
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return (false, errors, null);
            }
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

        public async Task<(string? Token, string? UserEmail)> GenerateEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (null, null); // User không tồn tại
            }

            // Gọi UserManager để tạo token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return (token, user.Email);
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            return await _userManager.GeneratePasswordResetTokenAsync(user);
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

        public async Task<UserIdentityDto?> GetUserByEmailAndValidateRefreshTokenAsync(string email, string refreshToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return null;
            }

            return new UserIdentityDto { Id = user.Id, Email = user.Email };
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

        public async Task<(bool IsSuccess, List<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (false, new List<string> { "No Accounts found with this email" });
            }

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

                if (result.Succeeded)
                {
                    return (true, new List<string>());
                }
                return (false, result.Errors.Select(e => e.Description).ToList());
            }
            catch (FormatException)
            {
                return (false, new List<string> { "Invalid token format." });
            }
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

        public async Task<(bool IsSuccess, List<string> Errors)> UpdateUserInfoAsync(string userId, UpdateInforDTO command)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, new List<string> { "User not found." });
            }

            // Cập nhật các thuộc tính
            user.PhoneNumber = command.PhoneNumber;
            user.Address = command.Address;
            user.Gender = command.Gender;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return (true, new List<string>());
            }
            return (false, result.Errors.Select(e => e.Description).ToList());
        }
    }
}
