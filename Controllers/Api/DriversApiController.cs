using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportRequestSystem.Data;
using TransportRequestSystem.Models;

namespace TransportRequestSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DriversApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriversApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDrivers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<object>());

            var drivers = await _context.Drivers
                .Where(d => d.IsActive)
                .Where(d => d.PersonnelNumber.Contains(query) || d.FullName.Contains(query))
                .Select(d => new
                {
                    id = d.Id,
                    personnelNumber = d.PersonnelNumber,
                    fullName = d.FullName,
                    phone = d.Phone
                })
                .Take(10)
                .ToListAsync();

            return Ok(drivers);
        }

        [HttpGet("vehicles/search")]
        public async Task<IActionResult> SearchVehicles([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<object>());

            var vehicles = await _context.Vehicles
                .Where(v => v.IsActive)
                .Where(v => v.PlateNumber.Contains(query) || v.Brand.Contains(query) || v.Model.Contains(query))
                .Select(v => new
                {
                    id = v.Id,
                    plateNumber = v.PlateNumber,
                    brand = v.Brand,
                    model = v.Model,
                    color = v.Color,
                    seatsCount = v.SeatsCount
                })
                .Take(10)
                .ToListAsync();

            return Ok(vehicles);
        }
    }
}