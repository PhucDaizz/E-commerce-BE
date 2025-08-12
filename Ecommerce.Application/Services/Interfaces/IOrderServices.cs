namespace Ecommerce.Application.Services.Interfaces
{
    public interface IOrderServices
    {
        Task<bool> CanncelOrderAsync(string orderId, string userId, bool isAdmin = false);
    }
}
