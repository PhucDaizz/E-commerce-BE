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
    }
}
