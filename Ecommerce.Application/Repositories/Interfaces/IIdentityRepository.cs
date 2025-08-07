using Ecommerce.Application.DTOS.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface IIdentityRepository
    {
        Task<LoginResponseDto> Login(LoginDTO user);
        Task<LoginResponseDto> RefreshToken(RefreshTokenModel model);
    }
}
