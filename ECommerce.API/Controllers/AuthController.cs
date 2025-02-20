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
        private readonly AuthDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IEmailServices emailServices;

        public AuthController(ITokenRepository tokenRepository, UserManager<ExtendedIdentityUser> userManager, AuthDbContext dbContext, IMapper mapper, IEmailServices emailServices)
        {
            this.tokenRepository = tokenRepository;
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.emailServices = emailServices;
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
        public async Task<IActionResult> RegisterAdmin([FromBody]RegisterAdminDTO registerAdminDTO)
        {
            var admin = new ExtendedIdentityUser
            {
                UserName = registerAdminDTO.Email?.Trim(),
                Email = registerAdminDTO.Email?.Trim(),
                PhoneNumber = registerAdminDTO.PhoneNumber?.Trim(),
            };

            var identityResult =  await userManager.CreateAsync(admin, registerAdminDTO.Password);

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
            await emailServices.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>;.", true);
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

            await emailServices.SendEmailAsync(forgotPasswordDTO.Email, "Reset Password", $"Please reset your password by <a href='{callback}'>clicking here</a>.", true);

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
        public async Task<IActionResult> UpdateInfor([FromBody]UpdateInforDTO updateInforDTO)
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

    }
}
