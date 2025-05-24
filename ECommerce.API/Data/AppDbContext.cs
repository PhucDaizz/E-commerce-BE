using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Models.Domain;
using System.Reflection.Emit;

namespace ECommerce.API.Data
{
    public class AppDbContext : IdentityDbContext<ExtendedIdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Bảng của Identity
        public DbSet<ExtendedIdentityUser> ExtendedIdentityUsers { get; set; }

        // Bảng của hệ thống ECommerce
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
        public DbSet<Payments> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Config Identity
            var superAdminRoleId = "07247d15-b6a9-4914-9fe9-1b6ee33e08ed";
            var adminRoleId = "477d3788-e4b3-4f3d-8dbd-aaead19b78ab";
            var userRoleId = "8bc05967-a01b-424c-a760-475af79c738f";

            // Create 3 roles SuperAdmin, Admin, User
            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = superAdminRoleId, Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            // Initial SuperadminUser
            var superAdminId = "808e47f5-a733-42ab-8e31-b6af349bfd90";
            var superAdminUser = new ExtendedIdentityUser
            {
                Id = superAdminId,
                UserName = "superadmin@ecommerce.com",
                NormalizedUserName = "superadmin@ecommerce.com".ToUpper(),
                Email = "superadmin@ecommerce.com",
                NormalizedEmail = "superadmin@ecommerce.com".ToUpper()
            };

            // Hash the password for the SuperAdmin user
            superAdminUser.PasswordHash = new PasswordHasher<ExtendedIdentityUser>().HashPassword(superAdminUser, "Superadmin@123");
            builder.Entity<ExtendedIdentityUser>().HasData(superAdminUser);

            // Assign roles to SuperAdminUser
            var superAdminRole = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId,
                },
                new IdentityUserRole<string> {
                    RoleId = adminRoleId,
                    UserId = superAdminId,
                },
                new IdentityUserRole<string> {
                    RoleId = userRoleId,
                    UserId = superAdminId,
                }
            };
            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRole);


            var paymentMethods = new List<PaymentMethods>
            {
                new PaymentMethods
                {
                    PaymentMethodID = 1,
                    MethodName = "VNPAY",
                    Description = "Ngân hàng, thanh toán trước khi đăt"
                },
                new PaymentMethods
                {
                    PaymentMethodID = 2,
                    MethodName = "COD",
                    Description = "Thông thường, thanh toán khi nhận hàng"
                }
            };
            builder.Entity<PaymentMethods>().HasData(paymentMethods);

            // Cấu hình các bảng khác
            builder.Entity<Shippings>()
                .HasOne(s => s.Orders)
                .WithMany(o => o.Shippings)
                .HasForeignKey(s => s.OrderID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Orders>()
                .HasOne(o => o.PaymentMethods)
                .WithMany(pm => pm.Orders)
                .HasForeignKey(o => o.PaymentMethodID)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình mối quan hệ giữa Orders và Shippings
            builder.Entity<Shippings>()
                .HasOne(s => s.Orders) // Một Shipping thuộc một đơn hàng
                .WithMany(o => o.Shippings) // Một đơn hàng có nhiều Shipping
                .HasForeignKey(s => s.OrderID) // Khóa ngoại ở bảng Shippings
                .OnDelete(DeleteBehavior.NoAction); // Tránh các vấn đề xóa cascade 

            // Cấu hình mối quan hệ giữa Orders và PaymentMethods
            builder.Entity<Orders>()
                 .HasOne(o => o.PaymentMethods)
                 .WithMany(pm => pm.Orders)
                 .HasForeignKey(o => o.PaymentMethodID)
                 .OnDelete(DeleteBehavior.NoAction); // Tránh các vấn đề xóa cascade 
            // Cấu hình mối quan hệ giữa OrderDetails và Orders
            builder.Entity<OrderDetails>()
                .HasOne(od => od.Orders)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.NoAction); // Tránh các vấn đề xóa cascade 

            builder.Entity<CartItems>()
                .HasOne(ci => ci.Products)  // CartItems có 1 Products
                .WithMany(p => p.CartItems) // Products có nhiều CartItems
                .HasForeignKey(ci => ci.ProductID) // Khóa ngoại là ProductID
                .OnDelete(DeleteBehavior.Restrict); // Tránh việc cascade delete

            // Cấu hình mối quan hệ giữa OrderDetails và Products
            /*modelBuilder.Entity<OrderDetails>() 
                .HasOne(od => od.Products) 
                .WithMany(p => p.OrderDetails) 
                .HasForeignKey(od => od.ProductID) 
                .OnDelete(DeleteBehavior.Restrict); */ // Tránh các vấn đề xóa cascade

            // Quan hệ CartItems -> ProductSizes
            builder.Entity<CartItems>()
                .HasOne(c => c.ProductSizes)
                .WithMany(ps => ps.CartItems)
                .HasForeignKey(c => c.ProductSizeID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductSizes>()
                .HasOne(ps => ps.ProductColors)
                .WithMany(pc => pc.ProductSizes)
                .HasForeignKey(ps => ps.ProductColorID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Orders>()
                .HasOne(o => o.Payments)
                .WithOne(p => p.Orders)
                .HasForeignKey<Payments>(p => p.OrderID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
