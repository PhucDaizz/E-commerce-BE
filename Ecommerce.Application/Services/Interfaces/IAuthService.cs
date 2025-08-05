using Ecommerce.Application.Common;
using Ecommerce.Application.DTOS.User;

namespace Ecommerce.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDTO request);

        Task<Result> RegisterUserAsync(RegisterRequestDto command);

        Task<Result> SendEmailConfirmationLinkAsync(string userId);
        Task<Result> SendPasswordResetLinkAsync(ForgotPasswordDTO command);
        Task<Result> ResetPasswordAsync(ResetPasswordDTO command);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenModel refreshTokenModel);

        Task<InforDTO?> GetInforAsync(string userId);

        Task<Result> ConfirmEmailAsync(string userId, string token);
    }
}
