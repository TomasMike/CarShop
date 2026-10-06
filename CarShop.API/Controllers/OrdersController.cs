using Microsoft.AspNetCore.Mvc;
using CarShop.Core.Services;
using CarShop.API.DTOs;

namespace CarShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Creates a new order for a car
        /// </summary>
        /// <param name="request">Order creation request containing car ID and customer ID</param>
        /// <returns>The created order</returns>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(request.CarId, request.CustomerId);
                var dto = MapToOrderDto(order);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific order by ID
        /// </summary>
        /// <param name="id">The order ID</param>
        /// <returns>The order details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { error = "Order not found" });

            var dto = MapToOrderDto(order);
            return Ok(dto);
        }

        /// <summary>
        /// Gets all orders for a specific customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>List of customer's orders</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetCustomerOrders(int customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            var dtos = orders.Select(MapToOrderDto).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Confirms a pending order
        /// </summary>
        /// <param name="id">The order ID</param>
        /// <returns>The confirmed order</returns>
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult<OrderDto>> ConfirmOrder(int id)
        {
            try
            {
                var order = await _orderService.ConfirmOrderAsync(id);
                var dto = MapToOrderDto(order);
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="id">The order ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            try
            {
                var success = await _orderService.CancelOrderAsync(id);
                if (!success)
                    return NotFound(new { error = "Order not found" });

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private static OrderDto MapToOrderDto(CarShop.Core.Models.Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CarId = order.CarId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalPrice = order.TotalPrice
            };
        }
    }
}
