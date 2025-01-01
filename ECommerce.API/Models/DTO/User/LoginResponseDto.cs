namespace ECommerce.API.Models.DTO.User
{
    public class LoginResponseDto
    {
        public string Email { get; set; }

        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
