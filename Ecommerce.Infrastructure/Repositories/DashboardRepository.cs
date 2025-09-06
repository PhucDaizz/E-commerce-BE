using Ecommerce.Application.DTOS.Dashboard;
using Ecommerce.Application.DTOS.ProductImage;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Enums;
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
            var now = DateTime.UtcNow;
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);

            var relevantOrders = await _dbContext.Orders
                .Where(o => o.Status == (int)OrderStatus.Confirmed && o.CreatedAt >= startOfLastMonth)
                .ToListAsync();

            var totalOrder = relevantOrders.Count;
            var totalOrderThisMonth = (float)relevantOrders.Count(o => o.CreatedAt >= startOfThisMonth);
            var totalOrderLastMonth = (float)relevantOrders.Count(o => o.CreatedAt >= startOfLastMonth && o.CreatedAt < startOfThisMonth);

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
            var now = DateTime.UtcNow;
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);

            var relevantOrders = await _dbContext.Orders
                .Where(o => o.Status == (int)OrderStatus.Confirmed || o.Status == (int)OrderStatus.Completed)
                .ToListAsync();

            var totalRevenue = relevantOrders.Sum(o => o.TotalAmount);

            var totalRevenueThisMonth = relevantOrders
                .Where(o => o.CreatedAt >= startOfThisMonth)
                .Sum(o => o.TotalAmount);

            var totalRevenueLastMonth = relevantOrders
                .Where(o => o.CreatedAt >= startOfLastMonth && o.CreatedAt < startOfThisMonth)
                .Sum(o => o.TotalAmount);

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

            var products = await _dbContext.Products
                .Where(p => topProductIds.Contains(p.ProductID))
                .Include(p => p.ProductImages) 
                .ToListAsync();

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

            return result.OrderByDescending(r => r.TotalSelling).ToList(); 
        }


        public async Task<List<DailyRevenueDTO>> FindDailyRevenue(DateTime startDate, DateTime endDate)
        {
            var dailyData = await _dbContext.Orders
                .Where(o => o.OrderDate.Date >= startDate.Date &&
                           o.OrderDate.Date <= endDate.Date &&
                           o.Status == 4)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

                var result = dailyData
                    .Select(x => new DailyRevenueDTO(x.Date, x.Revenue))
                    .OrderBy(d => d.Date)
                    .ToList();

                return result;
        }

        public async Task<List<MonthlyRevenueDTO>> FindMonthlyRevenue(int startYear, int startMonth, int endYear, int endMonth)
        {
            var startDate = new DateTime(startYear, startMonth, 1);
            var endDate = new DateTime(endYear, endMonth, 1).AddMonths(1).AddDays(-1);

            var monthlyData = await _dbContext.Orders
                .Where(o => o.OrderDate >= startDate &&
                           o.OrderDate <= endDate &&
                           o.Status == 4)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            var result = monthlyData
                .Select(x => new MonthlyRevenueDTO(x.Year, x.Month, x.Revenue))
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return result;
        }

        public async Task<List<CustomerLocationDTO>> GetTopCustomerLocationsAsync(int topN = 10)
        {
            var rawData = await _dbContext.Shippings
            .Include(s => s.Orders)
            .Where(s => s.Orders.Status == 4 && !string.IsNullOrEmpty(s.ShippingAddress))
            .Select(s => new { s.ShippingAddress, s.Orders.TotalAmount, s.Orders.UserID })
            .ToListAsync(); 

            var result = rawData
                .GroupBy(s => ExtractProvince(s.ShippingAddress))
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g => new CustomerLocationDTO
                {
                    Province = g.Key,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    CustomerCount = g.Select(x => x.UserID).Distinct().Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .ThenByDescending(x => x.TotalRevenue)
                .Take(topN)
                .ToList();

            return result;
        }

        public async Task<List<CustomerLocationDTO>> GetCustomerLocationsByProvinceAsync(string province)
        {
            var shippingData = await _dbContext.Shippings
                .Include(s => s.Orders)
                .Where(s => s.Orders.Status == 4 &&
                           !string.IsNullOrEmpty(s.ShippingAddress) &&
                           s.ShippingAddress.Contains(province))
                .Select(s => new
                {
                    s.ShippingAddress,
                    s.Orders.TotalAmount,
                    s.Orders.UserID
                })
                .ToListAsync();

            var result = shippingData
                .GroupBy(s => ExtractProvince(s.ShippingAddress))
                .Where(g => g.Key == province)
                .Select(g => new CustomerLocationDTO
                {
                    Province = g.Key,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    CustomerCount = g.Select(x => x.UserID).Distinct().Count()
                })
                .ToList();

            return result;
        }

        private string ExtractProvince(string address)
        {
            if (string.IsNullOrEmpty(address))
                return null;

            var parts = address.Split(',')
                .Select(part => part.Trim())
                .Where(part => !string.IsNullOrEmpty(part))
                .ToArray();

            if (parts.Length == 0)
                return null;

            var lastPart = parts[^1];

            return lastPart.Replace("Tỉnh", "").Replace("Thành phố", "").Trim();
        }
    }
}
