using CarShop.Context;
using CarShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private AppDbContext _context;
        public CarController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarAsync()
        {
            return await _context.Cars.ToListAsync();
        }

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
