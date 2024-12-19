using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.DTO.User
{
    public class RegisterAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string PhoneNumber { get; set; }
    }
}
