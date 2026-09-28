using Ecommerce.Application.Common;
using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Application.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Impemention
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailServices _emailServices;
        private readonly IOptions<FrontendSettings> _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IAuthRepository authRepository, ITokenGenerator tokenGenerator, 
                            IEmailServices emailServices, IOptions<FrontendSettings> configuration,
                            IUnitOfWork unitOfWork)
        {
            _authRepository = authRepository;
            _tokenGenerator = tokenGenerator;
            _emailServices = emailServices;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            var (isSuccess, errors) = await _authRepository.ConfirmEmailAsync(userId, token);

            if (!isSuccess)
            {
                return Result.Failure(errors);
            }

            return Result.Success();
        }

        public async Task<InforDTO?> GetInforAsync(string userId)
        {
            var user = await _authRepository.GetInforAsync(userId);
            return user;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDTO request)
        {
            var (isAuthenticated, user) = await _authRepository.AuthenticateUserAsync(request.Email, request.Password);
            if (!isAuthenticated) return new LoginResponseDto();
            var roles = await _authRepository.GetRolesAsync(user.Id);

            var createTokenDto = new CreateTokenDTO
            {
                Email = user.Email,
                UserId = user.Id,
            };
            var token = _tokenGenerator.CreateToken(createTokenDto, roles);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();
            await _authRepository.UpdateRefreshTokenAsync(user.Id, refreshToken);

            return new LoginResponseDto {Email = user.Email, Token = token, RefreshToken = refreshToken, Roles = roles };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenModel refreshTokenModel)
        {
            // 1. Giải mã access token cũ để lấy email một cách an toàn
            var principal = _tokenGenerator.GetPrincipalFromExpiredToken(refreshTokenModel.Token);
            var email = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return new LoginResponseDto(); 
            }

            // 2. Kiểm tra user và refresh token trong DB
            var user = await _authRepository.GetUserByEmailAndValidateRefreshTokenAsync(email, refreshTokenModel.RefreshToken);
            if (user == null)
            {
                return new LoginResponseDto();
            }

            var roles = await _authRepository.GetRolesAsync(user.Id);
            var createTokenDto = new CreateTokenDTO
            {
                Email = user.Email,
                UserId = user.Id,
            };
            var newAccessToken = _tokenGenerator.CreateToken(createTokenDto, roles);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            await _authRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken);
            await _unitOfWork.SaveChangesAsync(); 

            return new LoginResponseDto
            {
                Email = user.Email,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Roles = roles
            };
        }

        public async Task<Result> RegisterUserAsync(RegisterRequestDto command)
        {
            var existingUser = await _authRepository.FindByEmailAsync(command.Email);
            if (existingUser != null)
            {
                return Result.Failure("Email already exists.");
            }

            var (isSuccess, errors, newUser) = await _authRepository.CreateUserAsync(command);
            if (!isSuccess)
            {
                // Trả về một kết quả thất bại với danh sách lỗi từ Identity
                return Result.Failure(errors);
            }

            await _authRepository.AssignRoleAsync(newUser.Id, "User");
            await SendEmailConfirmationLinkAsync(newUser.Id);

            // Trả về một kết quả thành công
            return Result.Success();
        }

        public async Task<Result> SendEmailConfirmationLinkAsync(string userId)
        {
            var (token, userEmail) = await _authRepository.GenerateEmailConfirmationTokenAsync(userId);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userEmail))
            {
                return Result.Failure("Could not generate confirmation token. User may not exist.");
            }

            // 2. Mã hóa token để an toàn khi truyền qua URL
            // Dùng Base64UrlEncode an toàn hơn là Base64Encode thông thường
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

            // 3. Tạo link xác thực
            var frontendUrl = _configuration.Value.Url;
            if (string.IsNullOrEmpty(frontendUrl))
            {
                return Result.Failure("FrontendUrl is not configured.");
            }
            var confirmationLink = $"{frontendUrl}/verify-email?userId={userId}&token={encodedToken}";

            // 4. Tạo nội dung email
            // (Logic tạo HTML body nên được tách ra một lớp riêng nếu phức tạp)
            string subject = "Confirm Your Account - Ecommerce";
            string htmlBody = CreateConfirmationEmailBody(confirmationLink); // Hàm helper

            // 5. Gọi service để gửi email
            await _emailServices.SendEmailAsync(userEmail, subject, htmlBody, true);

            return Result.Success();
        }

        public async Task<Result> SendPasswordResetLinkAsync(ForgotPasswordDTO command)
        {
            var token = await _authRepository.GeneratePasswordResetTokenAsync(command.Email);
            if (token == null)
            {
                return Result.Failure("No Accounts found with this email");
            }

            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            var callbackUrl = QueryHelpers.AddQueryString(command.ClientUrl, "token", encodedToken);
            callbackUrl = QueryHelpers.AddQueryString(callbackUrl, "email", command.Email);

            string htmlBody = CreateResetPasswordEmailBody(callbackUrl);
            await _emailServices.SendEmailAsync(command.Email, "PhucDaiStore - Đặt lại mật khẩu", htmlBody, true);

            return Result.Success();
        }

        private string CreateConfirmationEmailBody(string confirmationLink)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Xác nhận tài khoản - Doris</title>
                </head>
                <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin:0; padding:0;"">
                    <div style=""width:100%; max-width:600px; margin:0 auto; background-color:#ffffff; padding:20px; box-shadow:0 0 10px rgba(0,0,0,0.05);"">
        
                        <!-- Logo / Header -->
                        <div style=""text-align:center; padding:20px 0; border-bottom:1px solid #eeeeee;"">
                            <img src=""https://iili.io/KoLNvF2.jpg"" alt=""Doris Logo"" style=""max-width:150px; margin-bottom:10px;"">
                            <h1 style=""color:#333333; font-size:22px; margin:0;"">Xác nhận tài khoản của bạn</h1>
                        </div>

                        <!-- Nội dung -->
                        <div style=""padding:30px 20px; text-align:center; color:#555555;"">
                            <p style=""font-size:16px; margin:0 0 15px;"">Xin chào Quý khách,</p>
                            <p style=""font-size:16px; margin:0 0 20px; line-height:1.6;"">
                                Cảm ơn bạn đã đăng ký tài khoản tại <strong>Doris</strong> – nền tảng mua sắm thời trang trực tuyến.  
                                Vui lòng xác nhận địa chỉ email của bạn bằng cách nhấn vào nút bên dưới:
                            </p>
            
                            <a href=""{confirmationLink}"" 
                                style=""display:inline-block; padding:12px 28px; background-color:#111111; color:#ffffff; 
                                        text-decoration:none; border-radius:30px; font-size:16px; font-weight:bold;"">
                                Xác nhận Email
                            </a>

                            <p style=""font-size:14px; margin:25px 0 0; color:#888888; line-height:1.6;"">
                                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
                            </p>
                        </div>

                        <!-- Footer -->
                        <div style=""text-align:center; padding:20px; font-size:12px; color:#999999; border-top:1px solid #eeeeee;"">
                            <p style=""margin:0;"">Trân trọng, <br> Đội ngũ Doris</p>
                            <p style=""margin:8px 0 0;"">
                                Hỗ trợ khách hàng: 
                                <a href=""mailto:support@doris.com"" style=""color:#111111; text-decoration:none;"">support@doris.com</a>
                            </p>
                            <p style=""margin:8px 0 0; font-size:11px; color:#bbbbbb;"">
                                © 2025 Doris. Tất cả các quyền được bảo lưu.
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        private string CreateResetPasswordEmailBody(string callback)
        {
            return $@"
                <!DOCTYPE html>
                    <html lang=""vi"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Đặt lại mật khẩu - Doris</title>
                    </head>
                    <body style=""font-family: Arial, sans-serif; background-color:#f4f4f4; margin:0; padding:0;"">
                        <div style=""width:100%; max-width:600px; margin:0 auto; background-color:#ffffff; padding:20px; box-shadow:0 0 10px rgba(0,0,0,0.05);"">
        
                            <!-- Logo / Header -->
                            <div style=""text-align:center; padding:20px 0; border-bottom:1px solid #eeeeee;"">
                                <img src=""https://iili.io/KoLNvF2.jpg"" alt=""Doris Logo"" style=""max-width:150px; margin-bottom:10px;"">
                                <h1 style=""color:#333333; font-size:22px; margin:0;"">Đặt lại mật khẩu của bạn</h1>
                            </div>

                            <!-- Nội dung -->
                            <div style=""padding:30px 20px; text-align:center; color:#555555;"">
                                <p style=""font-size:16px; margin:0 0 15px;"">Xin chào Quý khách,</p>
                                <p style=""font-size:16px; margin:0 0 20px; line-height:1.6;"">
                                    Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản tại <strong>Doris</strong>.  
                                    Vui lòng nhấn vào nút bên dưới để tạo mật khẩu mới:
                                </p>
            
                                <a href=""{callback}"" 
                                   style=""display:inline-block; padding:12px 28px; background-color:#111111; color:#ffffff; 
                                          text-decoration:none; border-radius:30px; font-size:16px; font-weight:bold;"">
                                    Đặt lại mật khẩu
                                </a>

                                <p style=""font-size:14px; margin:25px 0 0; color:#888888; line-height:1.6;"">
                                    Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.  
                                    Vì lý do bảo mật, liên kết sẽ hết hạn sau <strong>30 phút</strong>.
                                </p>
                            </div>

                            <!-- Footer -->
                            <div style=""text-align:center; padding:20px; font-size:12px; color:#999999; border-top:1px solid #eeeeee;"">
                                <p style=""margin:0;"">Trân trọng, <br> Đội ngũ Doris</p>
                                <p style=""margin:8px 0 0;"">
                                    Hỗ trợ khách hàng: 
                                    <a href=""mailto:support@doris.com"" style=""color:#111111; text-decoration:none;"">support@doris.com</a>
                                </p>
                                <p style=""margin:8px 0 0; font-size:11px; color:#bbbbbb;"">
                                    © 2025 Doris. Tất cả các quyền được bảo lưu.
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>

            ";
        }
       
    }
}
