using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.DTO.User
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? ClientUrl { get; set; }
    }
}
