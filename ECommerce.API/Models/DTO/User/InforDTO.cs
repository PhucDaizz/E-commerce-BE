namespace ECommerce.API.Models.DTO.User
{
    public class InforDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Gender { get; set; }
    }
}
