using Azure.Core;
using ECommerce.API.Data;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.User;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(ITokenRepository tokenRepository, UserManager<ExtendedIdentityUser> userManager, AuthDbContext dbContext)
        {
            this.tokenRepository = tokenRepository;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            /*var identityUser = await userManager.FindByEmailAsync(loginDTO.Email);
            if (identityUser != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, loginDTO.Password);
                if (checkPasswordResult)
                {
                    var roles = await userManager.GetRolesAsync(identityUser);
                    var jwtToken = tokenRepository.CreateToken(identityUser, roles.ToList());

                    var response = new LoginResponseDto
                    {
                        Email = loginDTO.Email,
                        Roles = roles.ToList(),
                        Token = jwtToken
                    };
                    return Ok(response);
                }
            }
            ModelState.AddModelError("", "Email or Password is not correct");
            return ValidationProblem(ModelState);
*/

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
                UserName = registerRequestDto.Email?.Trim(),
                Email = registerRequestDto.Email?.Trim(),
                PhoneNumber = registerRequestDto.PhoneNumber?.Trim()
            };

            var identityResult = await userManager.CreateAsync(user, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                identityResult = await userManager.AddToRoleAsync(user, "User");
                if (identityResult.Succeeded)
                {
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

    }
}
