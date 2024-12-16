using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ECommerce.API.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var superAdminRoleId = "07247d15-b6a9-4914-9fe9-1b6ee33e08ed";
            var adminRoleId = "477d3788-e4b3-4f3d-8dbd-aaead19b78ab";
            var userRoleId = "8bc05967-a01b-424c-a760-475af79c738f";

            // Create 3 roles SuperAdmin, Admin, User
            var role = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId,
                    Name = "SuperAdmin",
                    NormalizedName = "SuperAdmin".ToUpper()
                },
                new IdentityRole
                {
                    Id= adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole 
                {
                    Id= userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                }
            };
            builder.Entity<IdentityRole>().HasData(role);


            // Initial SuperadminUser
            var superAdminId = "808e47f5-a733-42ab-8e31-b6af349bfd90";
            var superAdminUser = new IdentityUser
            {
                Id = superAdminId,
                UserName = "superadmin@ecommerce.com",
                NormalizedUserName = "superadmin@ecommerce.com".ToUpper(),
                Email = "superadmin@ecommerce.com",
                NormalizedEmail = "superadmin@ecommerce.com".ToUpper()
            };

            // Hash the password for the SuperAdmin user
            superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(superAdminUser, "Superadmin@123");
            builder.Entity<IdentityUser>().HasData(superAdminUser);

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

        }

    }
}
