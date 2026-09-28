using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Services.Contracts.Infrastructure;
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
            // Validate JWT configuration
            if (string.IsNullOrEmpty(_jwtOptions.Value.Key))
            {
                throw new InvalidOperationException("JWT Key is not configured!");
            }

            if (string.IsNullOrEmpty(_jwtOptions.Value.Issuer))
            {
                throw new InvalidOperationException("JWT Issuer is not configured!");
            }

            if (string.IsNullOrEmpty(_jwtOptions.Value.Audience))
            {
                throw new InvalidOperationException("JWT Audience is not configured!");
            }

            Console.WriteLine("🔐 JWT Configuration Check:");
            Console.WriteLine($"   Key Length: {_jwtOptions.Value.Key.Length}");
            Console.WriteLine($"   Issuer: {_jwtOptions.Value.Issuer}");
            Console.WriteLine($"   Audience: {_jwtOptions.Value.Audience}");

            var now = DateTime.UtcNow;
            var expiration = now.AddHours(100000);

            var claims = new List<Claim>
            {
                // Standard JWT claims
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        
                // Custom claims for compatibility
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            // Debug output
            Console.WriteLine("\n🎫 Token Created Successfully:");
            Console.WriteLine($"   Issuer: {token.Issuer}");
            Console.WriteLine($"   Audience: {string.Join(", ", token.Audiences)}");
            Console.WriteLine($"   NotBefore: {token.ValidFrom:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"   Expires: {token.ValidTo:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"   Claims: {token.Claims.Count()}");
            Console.WriteLine($"   Roles: {string.Join(", ", roles)}");
            Console.WriteLine($"   Token Length: {tokenString.Length} characters");
            Console.WriteLine($"   Token Preview: {tokenString.Substring(0, Math.Min(50, tokenString.Length))}...\n");

            return tokenString;
        }

        public string GenerateRefreshToken()
        {
            var randomnumber = new  byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnumber);
            return Convert.ToBase64String(randomnumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key)),
                ValidateLifetime = false, 
                ValidIssuer = _jwtOptions.Value.Issuer,
                ValidAudience = _jwtOptions.Value.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
