using Microsoft.AspNetCore.Identity;

namespace ECommerce.API.Models.Domain
{
    public class ExtendedIdentityUser: IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool? Gender { get; set; }
        public string? Address { get; set; }
    }
}
