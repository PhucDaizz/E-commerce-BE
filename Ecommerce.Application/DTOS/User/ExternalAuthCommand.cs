using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOS.User
{
    public class ExternalAuthCommand
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Provider { get; set; } // "Google", "Facebook"
        public string ProviderKey { get; set; } // ID của user ở provider
    }
}
