using Ecommerce.Application.Common;
using Ecommerce.Application.DTOS.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ListUserDTO> ListUserAsync(string? querySearch, string searchField = "Email", int page = 1, int itemInPage = 10);

        Task<InforDTO?> GetInforAsync(string userId);
        Task<UserIdentityDto?> FindByEmailAsync(string email);
        Task<(bool IsSuccess, List<string> Errors, UserIdentityDto? NewUser)> CreateExternalUserAsync(ExternalAuthCommand command);
        Task AssignRoleAsync(string userId, string role);
        Task<bool> HasLoginAsync(string userId, string loginProvider);
        Task AddLoginAsync(string userId, string loginProvider, string providerKey);
        Task<List<string>> GetRolesAsync(string userId);
        Task UpdateRefreshTokenAsync(string userId, string refreshToken);
        Task<(bool IsAuthenticated, UserIdentityDto? User)> AuthenticateUserAsync(string email, string password);
        Task<(bool IsSuccess, List<string> Errors, UserIdentityDto? NewUser)> CreateUserAsync(RegisterRequestDto command);
        Task<(string? Token, string? UserEmail)> GenerateEmailConfirmationTokenAsync(string userId);
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<(bool IsSuccess, List<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword);
        Task<UserIdentityDto?> GetUserByEmailAndValidateRefreshTokenAsync(string email, string refreshToken);
        Task<(bool IsSuccess, List<string> Errors)> ConfirmEmailAsync(string userId, string token);
        Task<(bool IsSuccess, List<string> Errors)> UpdateUserInfoAsync(string userId, UpdateInforDTO command);
    }
}

