using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Repositories.Interface
{
    public interface IAuthRepository
    {
        Task<ListUserDTO> ListUserAsync(string? querySearch, string searchField = "Email" ,int page = 1, int itemInPage = 10);
    }
}
