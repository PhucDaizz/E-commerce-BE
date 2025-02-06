using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.User;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.API.Repositories.Impemention
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ExtendedIdentityUser> userManager;

        public TokenRepository(IConfiguration configuration, UserManager<ExtendedIdentityUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }


        public string CreateToken(ExtendedIdentityUser user, List<string> roles)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claim,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<LoginResponseDto> RefreshToken(RefreshTokenModel model)
        {
            var principal = GetTokenPrincipal(model.Token);

            var response = new LoginResponseDto();
            var email = principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return response;
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiry < DateTime.Now)
            {
                return response;
            }

            response.Email = user.Email;
            response.Roles = await userManager.GetRolesAsync(user);
            response.Token = CreateToken(user, response.Roles.ToList());
            response.RefreshToken = GenerateRefreshToken();

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(12);
            await userManager.UpdateAsync(user);

            return response;
        }




        private ClaimsPrincipal? GetTokenPrincipal(string token)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Value));

            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateLifetime = false,
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                ValidAudience = configuration.GetSection("Jwt:Audience").Value
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }


        private string GenerateRefreshToken()
        {
            var randomnumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnumber);
            return Convert.ToBase64String(randomnumber);
        }

        public async Task<LoginResponseDto> Login(LoginDTO user)
        {
            var response = new LoginResponseDto();
            var identityUser = await userManager.FindByEmailAsync(user.Email);
            if (identityUser == null || (await userManager.CheckPasswordAsync(identityUser, user.Password)) == false)
            {
                return response;
            }
            
            response.Email = user.Email;
            response.Roles = await userManager.GetRolesAsync(identityUser);
            response.Token = CreateToken(identityUser, response.Roles.ToList());
            response.RefreshToken = GenerateRefreshToken();

            identityUser.RefreshToken = response.RefreshToken;
            identityUser.RefreshTokenExpiry = DateTime.Now.AddDays(12);

            await userManager.UpdateAsync(identityUser);
            return response;
        }
    }
}
