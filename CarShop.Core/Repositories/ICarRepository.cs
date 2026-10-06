using CarShop.Core.Models;

namespace CarShop.Core.Repositories
{
    public interface ICarRepository
    {
        Task<Car?> GetByIdAsync(int id);
        Task<IEnumerable<Car>> GetAllAsync();
        Task<IEnumerable<Car>> GetAvailableCarsAsync();
        Task<Car> AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);
    }
}
