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
        private readonly UserManager<IdentityUser> userManager;

        public TokenRepository(IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }


        public string CreateToken(IdentityUser user, List<string> roles)
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
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
        /*public async Task<LoginResponseDto> RefreshToken(RefreshTokenModel model)
        {
            var principal = GetTokenPrincipal(model.Token);

            var response = new LoginResponseDto();
            if (principal?.Identity?.Name is null)
            {
                return response;
            }

            var user = await userManager.FindByEmailAsync(principal.Identity.Name);
            

        }*/



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

        
    }
}
