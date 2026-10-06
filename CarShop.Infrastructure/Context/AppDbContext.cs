using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarShop.Core.Models;

namespace CarShop.Context
{
    //dotnet ef database update  --project CarShop.Infrastructure --startup-project Carshop.API
    //dotnet ef migrations add ExpandCar  --project CarShop.Infrastructure --startup-project Carshop.API
    public class AppDbContext : IdentityDbContext<
        IdentityUser,                        // TUser
        IdentityRole,                        // TRole
        string,                              // TKey
        IdentityUserClaim<string>,           // TUserClaim
        IdentityUserRole<string>,            // TUserRole
        IdentityUserLogin<string>,           // TUserLogin
        IdentityRoleClaim<string>,           // TRoleClaim
        IdentityUserToken<string>>           // TUserToken
    {
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<CarBrand> CarBrands => Set<CarBrand>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Customer> Customers => Set<Customer>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "customer-role-id",
                Name = "Customer",
                NormalizedName = "CUSTOMER",
                ConcurrencyStamp = "aa55be3f-92a5-441a-917f-d28108a10ff4"
            });

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "admin-role-id",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "78f6f0fc-bf65-4e5b-9267-477ce7df7adc"
            });
        }
    }
}
