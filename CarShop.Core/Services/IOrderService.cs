using CarShop.Core.Models;

namespace CarShop.Core.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int carId, int customerId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
        Task<bool> CancelOrderAsync(int orderId);
        Task<Order> ConfirmOrderAsync(int orderId);
    }
}