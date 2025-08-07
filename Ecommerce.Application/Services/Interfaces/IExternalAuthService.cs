using Ecommerce.Application.DTOS.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IExternalAuthService
    {
        Task<AuthResultDto> AuthenticateAsync(ExternalAuthCommand command);
    }
}
