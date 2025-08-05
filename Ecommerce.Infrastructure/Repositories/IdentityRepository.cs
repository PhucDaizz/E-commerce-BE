using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Infrastructure.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<ExtendedIdentityUser> _userManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IOptions<JwtSettings> _jwtOptions;

        //TokenRepository cũ
        public IdentityRepository(UserManager<ExtendedIdentityUser> userManager, ITokenGenerator tokenGenerator, IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _jwtOptions = jwtOptions;
        }
        public async Task<LoginResponseDto> Login(LoginDTO request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            if (identityUser == null || !(await _userManager.CheckPasswordAsync(identityUser, request.Password)))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(identityUser);
            var createTokenDto = new CreateTokenDTO
            {
                Email = identityUser.Email,
                UserId = identityUser.Id,
            };

            var token = _tokenGenerator.CreateToken(createTokenDto, roles.ToList());
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            identityUser.RefreshToken = refreshToken;
            identityUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(12);
            await _userManager.UpdateAsync(identityUser);

            return new LoginResponseDto
            {
                Email = identityUser.Email,
                Roles = roles,
                Token = token,
                RefreshToken = refreshToken
            };
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

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiry < DateTime.Now)
            {
                return response;
            }

            response.Email = user.Email;
            response.Roles = await _userManager.GetRolesAsync(user);

            var createTokenDto = new CreateTokenDTO
            {
                Email = user.Email,
                UserId = user.Id,
            };
            response.Token = _tokenGenerator.CreateToken(createTokenDto, response.Roles.ToList());
            response.RefreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(12);
            await _userManager.UpdateAsync(user);

            return response;
        }

        private ClaimsPrincipal? GetTokenPrincipal(string token)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));

            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateLifetime = false,
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtOptions.Value.Issuer,
                ValidAudience = _jwtOptions.Value.Audience
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }
    }
}
