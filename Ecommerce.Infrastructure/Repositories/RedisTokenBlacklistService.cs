using Ecommerce.Application.Repositories.Interfaces;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Ecommerce.Infrastructure.Repositories
{
    public class RedisTokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDatabase _redisDatabase;
        private readonly IConnectionMultiplexer _redis;

        public RedisTokenBlacklistService(IConnectionMultiplexer redis)
        {
            _redisDatabase = redis.GetDatabase();
            _redis = redis;
        }
        public async Task<bool> AddToBlacklistAsync(string token, TimeSpan expiry)
        {
            var tokenHander = new JwtSecurityTokenHandler();
            var jwtToken = tokenHander.ReadJwtToken(token.Replace("Bearer ", ""));
            var tokenExpiry = jwtToken.ValidTo;

            var timeUntilExpiry = tokenExpiry - DateTime.UtcNow;

            return await _redisDatabase.StringSetAsync(
                GetBlacklistKey(token),
                "blacklisted",
                timeUntilExpiry > TimeSpan.Zero ? timeUntilExpiry : TimeSpan.FromMinutes(5));
        }

        public async Task<bool> IsBlacklistedAsync(string token)
        {
            return await _redisDatabase.KeyExistsAsync(GetBlacklistKey(token));
        }

        private static string GetBlacklistKey(string token)
        {
            var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(token)));
            return $"blacklist:{tokenHash}";
        }

        public async Task<bool> RemoveFromBlacklistAsync(string token)
        {
            return await _redisDatabase.KeyDeleteAsync(GetBlacklistKey(token));
        }
    }
}
