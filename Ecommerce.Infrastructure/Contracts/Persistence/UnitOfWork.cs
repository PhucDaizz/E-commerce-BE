using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;

namespace Ecommerce.Infrastructure.Contracts.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private bool _disposed;

        // repositories
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IProductColorRepository ProductColors { get; }
        public IProductImageRepository ProductImages { get; }
        public IProductSizeRepository ProductSizes { get; }
        public ICartItemRepository CartItems { get; }
        public IConversationRepository Conversation { get; }
        public IChatMessageRepository ChatMessage { get; }

        public UnitOfWork(
            AppDbContext dbContext,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IProductColorRepository productColorRepository,
            IProductImageRepository productImageRepository,
            IProductSizeRepository productSizeRepository,
            ICartItemRepository cartItemRepository,
            IConversationRepository conversationRepository, 
            IChatMessageRepository chatMessageRepository)
        {
            _dbContext = dbContext;
            Products = productRepository;
            Categories = categoryRepository;
            ProductColors = productColorRepository;
            ProductImages = productImageRepository;
            ProductSizes = productSizeRepository;
            CartItems = cartItemRepository;
            Conversation = conversationRepository;
            ChatMessage = chatMessageRepository;
        }

        public async Task BeginTransactionAsync()
        {
            await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
