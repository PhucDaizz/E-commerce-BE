using ECommerce.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CartItems> CartItems { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Discounts> Discounts { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductColors> ProductColors { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<ProductReviews> ProductReviews { get; set; }
        public DbSet<ProductSizes> ProductSizes { get; set; }
        public DbSet<Shippings> Shippings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ giữa Orders và Shippings
            modelBuilder.Entity<Shippings>()
                .HasOne(s => s.Orders) // Một Shipping thuộc một đơn hàng
                .WithMany(o => o.Shippings) // Một đơn hàng có nhiều Shipping
                .HasForeignKey(s => s.OrderID) // Khóa ngoại ở bảng Shippings
                .OnDelete(DeleteBehavior.NoAction); // Tránh các vấn đề xóa cascade 

            // Cấu hình mối quan hệ giữa Orders và PaymentMethods
            modelBuilder.Entity<Orders>()
                 .HasOne(o => o.PaymentMethods)
                 .WithMany(pm => pm.Orders)
                 .HasForeignKey(o => o.PaymentMethodID)
                 .OnDelete(DeleteBehavior.NoAction); // Tránh các vấn đề xóa cascade 
            // Cấu hình mối quan hệ giữa OrderDetails và Orders
            modelBuilder.Entity<OrderDetails>() 
                .HasOne(od => od.Orders) 
                .WithMany(o => o.OrderDetails) 
                .HasForeignKey(od => od.OrderID) 
                .OnDelete(DeleteBehavior.NoAction); // Tránh các vấn đề xóa cascade 

            modelBuilder.Entity<CartItems>()
                .HasOne(ci => ci.Products)
                .WithMany()
                .HasForeignKey(ci => ci.ProductID)
                .OnDelete(DeleteBehavior.Restrict);
            // Cấu hình mối quan hệ giữa OrderDetails và Products
            /*modelBuilder.Entity<OrderDetails>() 
                .HasOne(od => od.Products) 
                .WithMany(p => p.OrderDetails) 
                .HasForeignKey(od => od.ProductID) 
                .OnDelete(DeleteBehavior.Restrict); */ // Tránh các vấn đề xóa cascade

        }
    }   
}
