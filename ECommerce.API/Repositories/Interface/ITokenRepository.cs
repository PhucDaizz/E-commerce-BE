using Microsoft.AspNetCore.Identity;

namespace ECommerce.API.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateToken(IdentityUser user, List<string> roles);
    }
}
