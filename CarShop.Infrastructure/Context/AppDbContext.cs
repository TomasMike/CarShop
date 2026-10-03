using CarShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Context
{
    public class AppDbContext : IdentityDbContext //IdentityDbContext
    {
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<CarBrand> CarBrands => Set<CarBrand>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
