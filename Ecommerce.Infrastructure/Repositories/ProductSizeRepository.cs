using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ProductSizeRepository: IProductSizeRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductSizeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ProductSizes> CreateAsync(ProductSizes productSizes)
        {
            await _dbContext.ProductSizes.AddAsync(productSizes);
            await _dbContext.SaveChangesAsync();
            return productSizes;
        }

        public async Task<ProductSizes?> DeleteAsync(int ProductSizeID)
        {
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            _dbContext.ProductSizes.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<ProductSizes?> GetByIdAsync(int ProductSizeID)
        {
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            return existing;
        }

        public async Task<ProductSizes?> UpdateAsync(ProductSizes productSizes)
        {
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == productSizes.ProductSizeID);
            if (existing == null)
            {
                return null;
            }
            productSizes.UpdatedAt = DateTime.Now;
            _dbContext.ProductSizes.Entry(existing).CurrentValues.SetValues(productSizes);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<ProductSizes>?> GetAllByColorAsync(int id)
        {
            var productSizes = await _dbContext.ProductSizes.Where(x => x.ProductColorID == id).ToListAsync();
            if (!productSizes.Any())
            {
                return null;
            }
            return productSizes;
        }

        public async Task<bool> IsExistAsync(int productColorID, string size)
        {
            var existing = await _dbContext.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductColorID == productColorID && x.Size == size);

            return existing != null;
        }


        public async Task<ProductSizes?> AddAsync(ProductSizes productSizes)
        {
            var existing = await _dbContext.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductColorID == productSizes.ProductColorID && x.Size == productSizes.Size);

            if (existing != null)
            {
                existing.Stock += productSizes.Stock;
                existing.UpdatedAt = DateTime.Now;

                _dbContext.ProductSizes.Update(existing);
                await _dbContext.SaveChangesAsync();

                return existing;
            }

            return null;
        }

        public async Task<bool> DeleteByColorIDAsync(int colorID)
        {
            var existing = _dbContext.ProductSizes.Where(x => x.ProductColorID == colorID);
            if (!existing.Any())
            {
                return false;
            }
            _dbContext.ProductSizes.RemoveRange(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProductSizes?> DeleteByColorAndSizeAsync(int colorID, string size)
        {
            if (string.IsNullOrEmpty(size) || colorID == null)
            {
                return null;
            }
            var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductColorID == colorID && x.Size == size);
            if (existing == null)
            {
                return null;
            }
            _dbContext.ProductSizes.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<CartItems> cartItems)
        {
            List<ProductSizes> productSizesUpdate = new List<ProductSizes>();
            foreach (var item in cartItems)
            {
                var productSize = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == item.ProductSizeID);
                if (productSize == null)
                {
                    return false;
                }
                productSize.Stock -= item.Quantity;
                productSizesUpdate.Add(productSize);
            }
            _dbContext.ProductSizes.UpdateRange(productSizesUpdate);
            return true;
        }

        public async Task<bool> UpsertRangeAsync(IEnumerable<ProductSizes> productSizesToUpsert)
        {
            if (productSizesToUpsert == null || !productSizesToUpsert.Any())
            {
                return true;
            }

            // BƯỚC 1: Lấy tất cả các ProductColorID và Size từ danh sách đầu vào
            var uniqueKeys = productSizesToUpsert
                .Select(p => new { p.ProductColorID, p.Size })
                .Distinct();

            var productColorIds = uniqueKeys.Select(k => k.ProductColorID).ToList();
            var sizes = uniqueKeys.Select(k => k.Size).ToList();

            // BƯỚC 2: Tải tất cả các bản ghi có liên quan từ DB vào bộ nhớ trong một lần duy nhất
            var existingProductSizes = await _dbContext.ProductSizes
                .Where(ps => productColorIds.Contains(ps.ProductColorID) && sizes.Contains(ps.Size))
                .ToListAsync();

            // Chuyển sang Dictionary để tra cứu nhanh với key là "ProductColorID-Size"
            var existingMap = existingProductSizes.ToDictionary(ps => $"{ps.ProductColorID}-{ps.Size}");

            var sizesToInsert = new List<ProductSizes>();

            // BƯƠC 3: Lặp qua danh sách đầu vào và xử lý logic trong bộ nhớ
            foreach (var sizeToUpsert in productSizesToUpsert)
            {
                string key = $"{sizeToUpsert.ProductColorID}-{sizeToUpsert.Size}";

                // Nếu tìm thấy trong map (đã tồn tại trong DB)
                if (existingMap.TryGetValue(key, out var existingSize))
                {
                    // Logic cộng dồn Stock
                    existingSize.Stock = sizeToUpsert.Stock;
                    existingSize.UpdatedAt = DateTime.UtcNow;
                    // Không cần gọi .Update() vì entity đã được DbContext theo dõi
                }
                else // Nếu không tìm thấy (chưa tồn tại trong DB)
                {
                    // Thêm vào danh sách để AddRange
                    sizesToInsert.Add(sizeToUpsert);
                }
            }

            // BƯỚC 4: Thêm tất cả các bản ghi mới (nếu có)
            if (sizesToInsert.Any())
            {
                await _dbContext.ProductSizes.AddRangeAsync(sizesToInsert);
            }

            // BƯỚC 5: Lưu tất cả các thay đổi (cả UPDATE và INSERT) trong một giao dịch duy nhất
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> ReturnStockOnCancel(Dictionary<int, int> productSize)
        {
            foreach (var item in productSize)
            {
                var existing = await _dbContext.ProductSizes.FirstOrDefaultAsync(x => x.ProductSizeID == item.Key);
                if (existing != null)
                {
                    existing.Stock += item.Value;
                    existing.UpdatedAt = DateTime.Now;
                }
            }
            return true;
        }
    }
}
