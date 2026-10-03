using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarShop.Models;

namespace CarShop.Context
{
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

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
