namespace Ecommerce.Application.Repositories.Interfaces
{
    public interface ITokenBlacklistService
    {
        Task<bool> AddToBlacklistAsync(string token, TimeSpan expiry);
        Task<bool> IsBlacklistedAsync(string token);
        Task<bool> RemoveFromBlacklistAsync(string token);
    }
}
