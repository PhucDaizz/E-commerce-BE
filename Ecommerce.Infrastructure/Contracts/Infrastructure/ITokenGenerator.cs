using Ecommerce.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Contracts.Infrastructure
{
    public interface ITokenGenerator
    {
        string CreateToken(ExtendedIdentityUser user, List<string> roles);
        string GenerateRefreshToken();
    }
}
