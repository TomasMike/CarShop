using CarShop.Core.Models;
using CarShop.Core.Repositories;
using CarShop.Context;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly AppDbContext _context;

        public CarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarBrand)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars
                .Include(c => c.CarBrand)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync()
        {
            return await _context.Cars
                .Where(c => c.IsAvailable)
                .Include(c => c.CarBrand)
                .ToListAsync();
        }

        public async Task<Car> AddAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var car = await GetByIdAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }
    }
}
