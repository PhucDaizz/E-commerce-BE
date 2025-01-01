using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.User;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.API.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateToken(ExtendedIdentityUser user, List<string> roles);
        Task<LoginResponseDto> Login(LoginDTO user);
        Task<LoginResponseDto> RefreshToken(RefreshTokenModel model);

    }
}
