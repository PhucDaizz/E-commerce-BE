using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace ECommerce.API.Services.Impemention
{
    public class GoogleAuthServices: IGoogleAuthServices
    {
        private readonly UserManager<ExtendedIdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly IConfiguration _configuration;

        public GoogleAuthServices(
            UserManager<ExtendedIdentityUser> userManager,
            ITokenRepository tokenRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
        }


        public async Task HandleGoogleCallbackAsync(OAuthCreatingTicketContext context)
        {
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173"; // Lấy URL frontend từ config

            var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
            var name = context.Principal?.FindFirstValue(ClaimTypes.Name);
            var googleUserId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleUserId))
            {
                context.Fail("Email or Google User ID not found");
                return;
            }

            // Tìm user trong DB bằng email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Nếu user chưa tồn tại, tạo mới
                user = new ExtendedIdentityUser
                {
                    UserName = name ?? email, // Dùng email làm username nếu không có tên
                    Email = email,
                    EmailConfirmed = true // Email từ Google đã được xác thực
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(",", result.Errors.Select(e => e.Description));
                    context.Response.Redirect($"{frontendUrl}/login-failed?error={Uri.EscapeDataString(errors)}");
                    return;
                }
                // Thêm vai trò 'User' cho người dùng mới
                await _userManager.AddToRoleAsync(user, "User");
            }

            // Kiểm tra xem user đã liên kết với Google chưa
            var logins = await _userManager.GetLoginsAsync(user);
            if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == googleUserId))
            {
                // Nếu chưa, thêm liên kết
                await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googleUserId, "Google"));
            }

            // Tạo JWT và Refresh Token cho người dùng
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenRepository.CreateToken(user, roles.ToList());
            var refreshToken = _tokenRepository.GenerateRefreshToken(); // Gọi phương thức public đã sửa

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(12);
            await _userManager.UpdateAsync(user);

            context.Properties.Items["access_token"] = accessToken;
            context.Properties.Items["refresh_token"] = refreshToken;
        }
    }
}
