using AutoMapper;
using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Infrastructure;
using ECommerce.API.Models.Domain;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;

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
        private readonly IConfiguration _configuration;
        private readonly IExternalAuthService _externalAuthService;
        private readonly IAuthService _authService;

        public AuthController(ITokenRepository tokenRepository, 
            UserManager<ExtendedIdentityUser> userManager, 
            IMapper mapper, IEmailServices emailServices, IAuthRepository authRepository, 
            AppDbContext dbContext, IConfiguration configuration, IExternalAuthService externalAuthService, 
            IAuthService authService)
        {
            this.tokenRepository = tokenRepository;
            this.userManager = userManager;
            this.mapper = mapper;
            this.emailServices = emailServices;
            this.authRepository = authRepository;
            this.dbContext = dbContext;
            _configuration = configuration;
            _externalAuthService = externalAuthService;
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDTO);
            if (string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError("", "Email or Password is not correct");
                return ValidationProblem(ModelState);
            }
            return Ok(result);
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            return Challenge(
            new AuthenticationProperties { RedirectUri = "/api/auth/google-callback-handler" },
            "Google");
        }

        [HttpGet("google-callback-handler")]
        public async Task<IActionResult> GoogleSignInCallbackHandler()
        {
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";

            try
            {
                // Xác thực với Google
                var authenticateResult = await HttpContext.AuthenticateAsync("Google");

                if (!authenticateResult.Succeeded)
                {
                    return Redirect($"{frontendUrl}/login-failed?error=Google authentication failed");
                }

                // Lấy token từ Properties
                var accessToken = authenticateResult.Properties?.Items["access_token"];
                var refreshToken = authenticateResult.Properties?.Items["refresh_token"];
                var roles = authenticateResult.Properties?.Items["roles"];

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return Redirect($"{frontendUrl}/login-failed?error=Tokens not found");
                }

                // Redirect về frontend với token
                return Redirect($"{frontendUrl}/auth/callback?" +
                    $"token={accessToken}" +
                    $"&refreshToken={refreshToken}" +
                    $"&roles={roles}");
            }
            catch (Exception ex)
            {
                return Redirect($"{frontendUrl}/login-failed?error={Uri.EscapeDataString(ex.Message)}");
            }
        }


        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto registerRequestDto)
        {

            var result = await _authService.RegisterUserAsync(registerRequestDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Your account has been created");
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

            var result = await _authService.RefreshTokenAsync(refreshTokenModel);
            if (string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError("", "Invalid Token or Refresh Token");
                return ValidationProblem(ModelState);
            }

            return Ok(result);
        }

        [Route("GetnInfo")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _authService.GetInforAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return ValidationProblem(ModelState);
            }
            return Ok(user);
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


            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Ok("Email has been confirmed");
            }
            else
            {
                return BadRequest(new { Errors = result.Errors });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userInfo = await _authService.GetInforAsync(userId);
            if (userInfo == null)
            {
                return BadRequest("User not found");
            }

            await _authService.SendEmailConfirmationLinkAsync(userId);
            return Ok("Email has been sent");
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SendPasswordResetLinkAsync(forgotPasswordDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Email has been sent");
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authRepository.ResetPasswordAsync(resetPasswordDTO.Email, resetPasswordDTO.Token, resetPasswordDTO.Password);
            
            if (result.IsSuccess)
            {
                return Ok("Password has been reset");
            }
            else
            {
                var errors = result.Errors.Select(x => x);
                return BadRequest(new { Errors = errors });
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await authRepository.UpdateUserInfoAsync(userId, updateInforDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Errors = result.Errors });
            }
            return Ok("Your information has been updated.");
        }

        [HttpGet("GetInforById")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetInforByID([FromQuery]Guid id)
        {
            var user = await authRepository.GetInforAsync(id.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }
            return Ok(user);
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
