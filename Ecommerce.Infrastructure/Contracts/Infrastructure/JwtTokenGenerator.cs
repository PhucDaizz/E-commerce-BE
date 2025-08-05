using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Infrastructure.Contracts.Infrastructure
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IOptions<JwtSettings> _jwtOptions;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }
        public string CreateToken(CreateTokenDTO user, List<string> roles)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId)
            };
            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claim,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomnumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnumber);
            return Convert.ToBase64String(randomnumber);
        }
    }
}
