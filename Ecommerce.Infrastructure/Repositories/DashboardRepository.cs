using Ecommerce.Application.DTOS.Dashboard;
using Ecommerce.Application.DTOS.ProductImage;
using Ecommerce.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _dbContext;

        public DashboardRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ReportInventoryDTO> GetReportInventoryAsync()
        {
            var totalInventory = await _dbContext.ProductSizes.SumAsync(p => p.Stock);
            var totalProductActive = await _dbContext.Products.CountAsync(p => p.IsPublic);
            var totalProduct = await _dbContext.Products.CountAsync();
            return new ReportInventoryDTO
            {
                TotalInventory = totalInventory,
                TotalProductActive = totalProductActive,
                TotalProduct = totalProduct
            };
        }

        public async Task<ReportOrderDTO> GetReportOrderAsync()
        {
            var totalOrder = await _dbContext.Orders.CountAsync();
            var now = DateTime.UtcNow;
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddDays(-1);

            var totalOrderThisMonth = (float)await _dbContext.Orders
                .CountAsync(o => o.CreatedAt >= startOfThisMonth && o.CreatedAt <= endOfThisMonth);

            var totalOrderLastMonth = (float)await _dbContext.Orders
                .CountAsync(o => o.CreatedAt >= startOfLastMonth && o.CreatedAt <= endOfLastMonth);

            var orderChangePercentage = totalOrderLastMonth == 0 ? 100 : ((totalOrderThisMonth - totalOrderLastMonth) / totalOrderLastMonth) * 100;

            return new ReportOrderDTO
            {
                TotalOrder = totalOrder,
                OrderChangePercentage = orderChangePercentage,
                TotalOrderThisMonth = Convert.ToInt32(totalOrderThisMonth)
            };
        }

        public async Task<List<ReportTopGender>> GetReportTopGenderAsync()
        {
            return await _dbContext.Database
            .SqlQueryRaw<ReportTopGender>(
                @"SELECT 
                    u.Gender AS Gender,
                    COUNT(DISTINCT o.UserID) AS TotalUsers, 
                    COUNT(o.OrderID) AS TotalOrders
                FROM Orders o
                JOIN AspNetUsers u ON o.UserID = u.Id
                GROUP BY u.Gender")
            .ToListAsync();
        }

        public async Task<ReportUserDTO> GetReportUserAsync()
        {
            var totalUser = await _dbContext.Users.CountAsync();
            return new ReportUserDTO
            {
                TotalUser = totalUser
            };
        }

        public async Task<SalesRevenueDTO> GetSalesRevenueAsync()
        {
            var totalRevenue = await _dbContext.Orders.SumAsync(o => o.TotalAmount);
            var now = DateTime.UtcNow;
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddDays(-1);

            var totalRevenueThisMonth = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startOfThisMonth && o.CreatedAt <= endOfThisMonth)
                .SumAsync(o => o.TotalAmount);

            var totalRevenueLastMonth = await _dbContext.Orders
                .Where(o => o.CreatedAt >= startOfLastMonth && o.CreatedAt <= endOfLastMonth)
                .SumAsync(o => o.TotalAmount);

            var revenueChangePercentage = totalRevenueLastMonth == 0 ? 100 : ((totalRevenueThisMonth - totalRevenueLastMonth) / totalRevenueLastMonth) * 100;

            return new SalesRevenueDTO
            {
                TotalRevenue = totalRevenue,
                RevenueChangePercentage = revenueChangePercentage,
                TotalRevenueThisMonth = totalRevenueThisMonth
            };
        }

        public async Task<List<ReportTopSellingProductDTO>> TopSellingProductsAsync(int items)
        {
            // Bước 1: Lấy top sản phẩm bán chạy
            var topProductsInfo = await _dbContext.OrderDetails
                .GroupBy(od => od.ProductID)
                .Select(group => new
                {
                    ProductID = group.Key,
                    TotalQuantity = group.Sum(x => x.Quantity)
                })
                .OrderByDescending(result => result.TotalQuantity)
                .Take(items)
                .ToListAsync();

            var topProductIds = topProductsInfo.Select(p => p.ProductID).ToList();

            // Bước 2: Lấy thông tin chi tiết và ảnh của các sản phẩm đó
            var products = await _dbContext.Products
                .Where(p => topProductIds.Contains(p.ProductID))
                .Include(p => p.ProductImages) // Dùng Include hiệu quả hơn
                .ToListAsync();

            // Bước 3: Kết hợp lại trong bộ nhớ
            var result = (from info in topProductsInfo
                          join product in products on info.ProductID equals product.ProductID
                          select new ReportTopSellingProductDTO
                          {
                              ProductID = product.ProductID,
                              ProductName = product.ProductName,
                              CategoryID = product.CategoryID,
                              Price = product.Price,
                              IsPublic = product.IsPublic,
                              TotalSelling = info.TotalQuantity,
                              Images = product.ProductImages.Select(img => new ProductImageDTO
                              {
                                  ImageID = img.ImageID,
                                  ImageURL = img.ImageURL,
                                  IsPrimary = img.IsPrimary
                              }).ToList()
                          }).ToList();

            return result.OrderByDescending(r => r.TotalSelling).ToList(); // Sắp xếp lại vì Join có thể làm thay đổi thứ tự
        }
    }
}
