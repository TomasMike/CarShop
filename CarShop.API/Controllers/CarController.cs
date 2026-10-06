using CarShop.Context;
using CarShop.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Controllers
{
    /// <summary>
    /// Controller for managing car operations in the CarShop API.
    /// Provides endpoints for retrieving and creating cars.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the CarController class.
        /// </summary>
        /// <param name="context">The application database context for accessing car data.</param>
        public CarController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all cars from the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// Returns an IEnumerable of Car objects if successful.
        /// Returns a 200 OK response with the list of all cars.
        /// </returns>
        /// <remarks>
        /// This endpoint does not require authentication.
        /// Example: GET /car
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific car by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the car to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// Returns the Car object with the specified ID if found.
        /// Returns a 200 OK response with the car details if successful.
        /// Returns a 404 Not Found response if no car with the specified ID exists.
        /// </returns>
        /// <remarks>
        /// This endpoint does not require authentication.
        /// Example: GET /car/1
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCarByIdAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return car;
        }

        /// <summary>
        /// Creates a new car and adds it to the database.
        /// </summary>
        /// <param name="car">The Car object containing the details of the car to be added.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// Returns the newly created Car object with the generated ID.
        /// Returns a 200 OK response with the saved car details if successful.
        /// Returns a 400 Bad Request response if the car object is invalid.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authentication via Bearer token in the Authorization header.
        /// The car object must contain:
        /// - Name (string, required): The name of the car
        /// - Price (double, required): The price of the car
        /// Example: POST /car with JSON body { "name": "Tesla Model 3", "price": 45000 }
        /// 
        /// Required Header:
        /// Authorization: Bearer {token}
        /// </remarks>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Car>> AddCar(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return Ok(car);
        }

    }
}
