using ECommerce.API.Data;
using ECommerce.API.Models.DTO.Dashboard;
using ECommerce.API.Models.DTO.Product;
using ECommerce.API.Models.DTO.ProductImage;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ECommerce.API.Services.Impemention
{
    public class DashboardServices : IDashboardServices
    {
        private readonly ECommerceDbContext dbContext;
        private readonly AuthDbContext authDbContext;

        public DashboardServices(ECommerceDbContext dbContext, AuthDbContext authDbContext)
        {
            this.dbContext = dbContext;
            this.authDbContext = authDbContext;
        }

        public async Task<ReportInventoryDTO> GetReportInventoryAsync()
        {
            var totalInventory = await dbContext.ProductSizes.Select(p => p.Stock).SumAsync();
            var totalProductActive = await dbContext.Products.Where(p => p.IsPublic == true).CountAsync();
            var totalProduct = await dbContext.Products.CountAsync();
            return new ReportInventoryDTO
            {
                TotalInventory = totalInventory,
                TotalProductActive = totalProductActive,
                TotalProduct = totalProduct
            };
        }

        public async Task<ReportOrderDTO> GetReportOrderAsync()
        {
            var totalOrder = await dbContext.Orders.CountAsync();

            var startOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);

            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddDays(-1);

            var totalOrderThisMonth = (float)await dbContext.Orders
                .Where(o => o.CreatedAt >= startOfThisMonth && o.CreatedAt <= endOfThisMonth)
                .CountAsync();

            var totalOrderLastMonth =  (float)await dbContext.Orders
                .Where(o => o.CreatedAt >= startOfLastMonth && o.CreatedAt <= endOfLastMonth)
                .CountAsync();

            var OrderChangePercentage = totalOrderLastMonth == 0 ? 0 : (float)((totalOrderThisMonth - totalOrderLastMonth) / totalOrderLastMonth) * 100;

            return new ReportOrderDTO
            {
                TotalOrder = totalOrder,
                OrderChangePercentage = OrderChangePercentage,
                TotalOrderThisMonth = Convert.ToInt32(totalOrderThisMonth)
            };
        }

        public async Task<List<ReportTopGender>> GetReportTopGenderAsync()
        {
            var genderStats = await dbContext.Database
                .SqlQueryRaw<ReportTopGender>(
                    @"SELECT 
                        u.Gender AS Gender,
                        COUNT(DISTINCT o.UserID) AS TotalUsers, 
                        COUNT(o.OrderID) AS TotalOrders
                    FROM Orders o
                    JOIN AspNetUsers u ON o.UserID = u.Id
                    GROUP BY u.Gender")
                    .ToListAsync();

            return genderStats.Any() ? genderStats : new List<ReportTopGender>();
        }


        public async Task<ReportUserDTO> GetReportUserAsync()
        {
            var totalUser = await authDbContext.Users.CountAsync();
            return new ReportUserDTO
            {
                TotalUser = totalUser
            };

        }

        public async Task<SalesRevenueDTO> GetSalesRevenueAsync()
        {
            var totalRevenue = await dbContext.Orders.SumAsync(o => o.TotalAmount);

            var startOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.UtcNow.Month, 1);
            var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);

            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddDays(-1);

            var totalRevenueThisMonth = await dbContext.Orders
                .Where(o => o.CreatedAt >= startOfThisMonth && o.CreatedAt <= endOfThisMonth)
                .SumAsync(o => o.TotalAmount);

            var totalRevenueLastMonth = await dbContext.Orders
                .Where(o => o.CreatedAt >= startOfLastMonth && o.CreatedAt <= endOfLastMonth)
                .SumAsync(o => o.TotalAmount);

            var revenueChangePercentage = totalRevenueLastMonth == 0 ? 0 : ((totalRevenueThisMonth - totalRevenueLastMonth) / totalRevenueLastMonth) * 100;

            return new SalesRevenueDTO
            {
                TotalRevenue = totalRevenue,
                RevenueChangePercentage = revenueChangePercentage,
                TotalRevenueThisMonth = totalRevenueThisMonth
            };
        }

        public async Task<List<ReportTopSellingProductDTO>> TopSellingProducts(int items)
        {
            var productList = await dbContext.OrderDetails
                .GroupBy(od => od.ProductID)
                .Select(group => new
                {
                    ProductID = group.Key,
                    TotalQuantity = group.Sum(x => x.Quantity)
                })
                .OrderByDescending(result => result.TotalQuantity)
                .Take(items)
                .Join(dbContext.Products,
                      od => od.ProductID,
                      p => p.ProductID,
                      (od, p) => new ReportTopSellingProductDTO
                      {
                          ProductID = p.ProductID,
                          ProductName = p.ProductName,
                          CategoryID = p.CategoryID,
                          Price = p.Price,
                          IsPublic = p.IsPublic,
                          TotalSelling = od.TotalQuantity,
                          Images = dbContext.ProductImages
                              .Where(img => img.ProductID == p.ProductID)
                              .Select(img => new ProductImageDTO
                              {
                                  ImageID = img.ImageID,
                                  ImageURL = img.ImageURL,
                                  IsPrimary = img.IsPrimary
                              })
                              .ToList()
                      })
                .ToListAsync();

            return productList;
        }
    }
}
