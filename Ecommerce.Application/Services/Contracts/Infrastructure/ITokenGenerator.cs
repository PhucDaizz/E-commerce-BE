using Ecommerce.Application.DTOS.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Contracts.Infrastructure
{
    public interface ITokenGenerator
    {
        string CreateToken(CreateTokenDTO user, List<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
