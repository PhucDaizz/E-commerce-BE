using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Interfaces;

namespace Ecommerce.Application.Services.Impemention
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public ExternalAuthService(IAuthRepository authRepository, ITokenGenerator tokenGenerator, IUnitOfWork unitOfWork)
        {
            _authRepository = authRepository;
            _tokenGenerator = tokenGenerator;
            _unitOfWork = unitOfWork;
        }
        public async Task<AuthResultDto> AuthenticateAsync(ExternalAuthCommand command)
        {
            var userIdentity = await _authRepository.FindByEmailAsync(command.Email);
            bool isNewUser = false;

            // 2. Nếu chưa có, tạo mới
            if (userIdentity == null)
            {
                var (isSuccess, errors, newUser) = await _authRepository.CreateExternalUserAsync(command); // Không có password
                if (!isSuccess)
                {
                    return new AuthResultDto { IsSuccess = false, ErrorMessage = string.Join(", ", errors) };
                }
                userIdentity = newUser;
                isNewUser = true;
            }

            // 3. Gán role nếu là user mới
            if (isNewUser)
            {
                await _authRepository.AssignRoleAsync(userIdentity.Id, "User");
            }

            // 4. Thêm thông tin đăng nhập ngoài (Google)
            var hasLogin = await _authRepository.HasLoginAsync(userIdentity.Id, command.Provider);
            if (!hasLogin)
            {
                await _authRepository.AddLoginAsync(userIdentity.Id, command.Provider, command.ProviderKey);
            }

            // 5. Tạo JWT và Refresh Token
            var crateUserToken = new CreateTokenDTO
            {
                Email = userIdentity.Email,
                UserId = userIdentity.Id,
            };
            var roles = await _authRepository.GetRolesAsync(userIdentity.Id);

            var accessToken = _tokenGenerator.CreateToken(crateUserToken, roles);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();


            await _authRepository.UpdateRefreshTokenAsync(userIdentity.Id, refreshToken);

            await _unitOfWork.SaveChangesAsync();

            return new AuthResultDto
            {
                IsSuccess = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Roles = roles
            };
        }
    }
}
