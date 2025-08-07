using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.User
{
    public class CreateTokenDTO
    {
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}
