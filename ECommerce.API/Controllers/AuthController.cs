using AutoMapper;
using Azure.Core;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.User;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenRepository tokenRepository;
        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly IEmailServices emailServices;
        private readonly IAuthRepository authRepository;
        private readonly AppDbContext dbContext;

        public AuthController(ITokenRepository tokenRepository, UserManager<ExtendedIdentityUser> userManager, IMapper mapper, IEmailServices emailServices, IAuthRepository authRepository, AppDbContext dbContext)
        {
            this.tokenRepository = tokenRepository;
            this.userManager = userManager;
            this.mapper = mapper;
            this.emailServices = emailServices;
            this.authRepository = authRepository;
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var loginResult = await tokenRepository.Login(loginDTO);
            if (string.IsNullOrEmpty(loginResult.Token))
            {
                ModelState.AddModelError("", "Email or Password is not correct");
                return ValidationProblem(ModelState);
            }
            return Ok(loginResult);
        }


        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto registerRequestDto)
        {
            var user = new ExtendedIdentityUser
            {
                UserName = registerRequestDto.UserName?.Trim(),
                Email = registerRequestDto.Email?.Trim(),
                PhoneNumber = registerRequestDto.PhoneNumber?.Trim()
            };

            var identityResult = await userManager.CreateAsync(user, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                identityResult = await userManager.AddToRoleAsync(user, "User");
                if (identityResult.Succeeded)
                {
                    await GenerateEmailConfirmationToken(user, registerRequestDto.Email);
                    return Ok("Your account have been created");
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            // Account has been created
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }

        [HttpPost]
        [Route("RegisterAdmin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequestDto registerAdminDTO)
        {
            var admin = new ExtendedIdentityUser
            {
                UserName = registerAdminDTO.UserName?.Trim(),
                Email = registerAdminDTO.Email?.Trim(),
                PhoneNumber = registerAdminDTO.PhoneNumber?.Trim(),
            };

            var identityResult = await userManager.CreateAsync(admin, registerAdminDTO.Password);

            if (identityResult.Succeeded)
            {
                string[] roles = ["Admin", "User"];
                identityResult = await userManager.AddToRolesAsync(admin, roles);

                if (identityResult.Succeeded)
                {
                    return Ok("Your account have been created");
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }


        [Route("Refreshtoken")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel refreshTokenModel)
        {
            var loginResult = await tokenRepository.RefreshToken(refreshTokenModel);
            if (string.IsNullOrEmpty(loginResult.Token))
            {
                ModelState.AddModelError("", "Invalid Token or Refresh Token");
                return ValidationProblem(ModelState);
            }
            return Ok(loginResult);
        }

        [Route("GetnInfo")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInfo()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return ValidationProblem(ModelState);
            }

            var result = mapper.Map<InforDTO>(user);

            return Ok(result);
        }


        [HttpGet]
        [Route("email-confirmation")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError("", "User Id and Token are required");
                return ValidationProblem(ModelState);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return ValidationProblem(ModelState);
            }

            var decodedToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Ok("Email has been confirmed");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return ValidationProblem(ModelState);
            }
        }

        private async Task GenerateEmailConfirmationToken(ExtendedIdentityUser user, string? email)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"http://localhost:5173/verifyemail?userId={user.Id}&token={encodedToken}";

            string htmlBody = $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Xác nhận tài khoản của bạn</title>
                </head>
                <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;"">
                    <div style=""width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
                        <!-- Tiêu đề -->
                        <div style=""text-align: center; padding: 20px 0;"">
                            <h1 style=""color: #333333; font-size: 24px; margin: 0;"">Xác nhận tài khoản của bạn</h1>
                        </div>
            
                        <!-- Nội dung chính -->
                        <div style=""padding: 20px; text-align: center;"">
                            <p style=""color: #666666; font-size: 16px; line-height: 1.5;"">Kính gửi quý khách,</p>
                            <p style=""color: #666666; font-size: 16px; line-height: 1.5;"">Để hoàn tất quá trình đăng ký tài khoản của bạn, vui lòng xác nhận địa chỉ email bằng cách nhấp vào nút bên dưới:</p>
                            <a href=""{confirmationLink}"" style=""display: inline-block; padding: 10px 20px; background-color: #007BFF; color: #ffffff; text-decoration: none; border-radius: 5px; font-size: 16px; margin: 20px 0;"">Xác nhận email</a>
                            <p style=""color: #666666; font-size: 16px; line-height: 1.5;"">Nếu bạn không thực hiện yêu cầu đăng ký này, xin vui lòng bỏ qua email.</p>
                        </div>
            
                        <!-- Footer -->
                        <div style=""text-align: center; padding: 20px; font-size: 12px; color: #999999;"">
                            <p>Trân trọng,<br>[Tên công ty của bạn]</p>
                            <p>Nếu bạn cần hỗ trợ, vui lòng liên hệ với chúng tôi qua <a href=""mailto:support@yourcompany.com"" style=""color: #007BFF; text-decoration: none;"">support@yourcompany.com</a>.</p>
                        </div>
                    </div>
                </body>
                </html>
                ";


            await emailServices.SendEmailAsync(email, "PhucDaiStore - Xác nhận eamil của bạn", htmlBody, true);
        }

        [Authorize]
        [HttpPost]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmail()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            await GenerateEmailConfirmationToken(user, user.Email);
            return Ok("Email has been sent");
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                return BadRequest("No Accounts found with this email");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));

            var param = new Dictionary<string, string>
            {
                {"token", encodedToken},
                {"email", forgotPasswordDTO.Email}
            };

            var callback = QueryHelpers.AddQueryString(forgotPasswordDTO.ClientUrl, param);

            string htmlBody = $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Đặt lại mật khẩu của bạn</title>
                </head>
                <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;"">
                    <div style=""width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
                        <div style=""text-align: center; padding: 20px 0;"">
                            <h1 style=""color: #333333; font-size: 24px; margin: 0;"">Đặt lại mật khẩu của bạn</h1>
                        </div>
                        <div style=""padding: 20px; text-align: center;"">
                            <p style=""color: #666666; font-size: 16px; line-height: 1.5;"">Kính gửi quý khách,</p>
                            <p style=""color: #666666; font-size: 16px; line-height: 1.5;"">Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình. Vui lòng nhấp vào nút bên dưới để tiến hành:</p>
                            <a href=""{callback}"" style=""display: inline-block; padding: 10px 20px; background-color: #007BFF; color: #ffffff; text-decoration: none; border-radius: 5px; font-size: 16px; margin: 20px 0;"">Đặt lại mật khẩu</a>
                            <p style=""color: #666666; font-size: 16px; line-height: 1.5;"">Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                        </div>
                        <div style=""text-align: center; padding: 20px; font-size: 12px; color: #999999;"">
                            <p>Trân trọng,<br>[Tên công ty của bạn]</p>
                            <p>Nếu bạn cần hỗ trợ, vui lòng liên hệ qua <a href=""mailto:support@yourcompany.com"" style=""color: #007BFF; text-decoration: none;"">support@yourcompany.com</a>.</p>
                        </div>
                    </div>
                </body>
                </html>
                ";

            await emailServices.SendEmailAsync(forgotPasswordDTO.Email, "PhucDaiStore - Đặt lại mật khẩu", htmlBody, true);

            return Ok("Email has been sent");
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return BadRequest("No Accounts found with this email");
            }

            var decodedToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(resetPasswordDTO.Token));
            var result = await userManager.ResetPasswordAsync(user, decodedToken!, resetPasswordDTO.Password!);

            if (result.Succeeded)
            {
                return Ok("Password has been reset");
            }
            else
            {
                var erroes = result.Errors.Select(x => x.Description);
                return BadRequest(new { Errors = erroes });
            }
        }


        [HttpPost("updateinfor")]
        [Authorize]
        public async Task<IActionResult> UpdateInfor([FromBody] UpdateInforDTO updateInforDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            user.PhoneNumber = updateInforDTO.PhoneNumber;
            user.Address = updateInforDTO.Address;
            user.Gender = updateInforDTO.Gender;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Your information has been updated");
            }
            else
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new { Errors = errors });
            }
        }

        [HttpGet("GetInforById")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetInforByID([FromQuery]Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var result = mapper.Map<InforDTO>(user);
            return Ok(result);
        }

        [HttpGet("GetAllUser")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllUser([FromQuery]string? querySearch,[FromQuery]string searchField = "Email",[FromQuery]int page = 1,[FromQuery]int itemInPage = 10)
        {
            var users = await authRepository.ListUserAsync(querySearch,searchField,page,itemInPage);
            return Ok(users);
        }
    }
}
