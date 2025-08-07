using Ecommerce.Application.Repositories.Interfaces;

namespace Ecommerce.Application.Repositories.Persistence
{
    public interface IUnitOfWork: IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IProductColorRepository ProductColors { get; }
        IProductImageRepository ProductImages { get; }
        IProductSizeRepository ProductSizes { get; }
        ICartItemRepository CartItems { get; }
        IConversationRepository Conversation { get; }
        IChatMessageRepository ChatMessage { get; }
        IDiscountRepository Discounts { get; }
        IOrderRepository Orders { get; }
        IShippingRepository shipping { get; }
        IOrderDetailRepository OrderDetails { get; }
        IPaymentRepository Payment { get; }
        IBannerRepository Banners { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
