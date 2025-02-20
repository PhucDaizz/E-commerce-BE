using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.DTO.User
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(25)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name can only contain letters and spaces.")]
        public string UserName { get; set; }

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
