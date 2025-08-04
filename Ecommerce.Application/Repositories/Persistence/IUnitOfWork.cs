using Ecommerce.Application.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
