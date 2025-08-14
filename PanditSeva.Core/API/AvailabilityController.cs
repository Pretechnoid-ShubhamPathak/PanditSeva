// Controllers/AvailabilityController.cs
using Data.DTOs;
using Data.Models;
using Data.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace PanditSeva.Core.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {

        private readonly AppDbContext _db;
        public AvailabilityController(AppDbContext db) => _db = db;

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int priestProfileId, [FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null)
        {
            var q = _db.Availabilities.AsQueryable().Where(a => a.PriestId == priestProfileId && a.IsAvailable);
            if (from is not null) q = q.Where(a => a.Date >= from);
            if (to is not null) q = q.Where(a => a.Date <= to);

            var list = await q.OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToListAsync();
            return Ok(list);
        }

        // Priests/Admin create a slot
        [Authorize(Roles = "Priest,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAvailabilityDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime.");

            var exists = await _db.PriestProfiles.AnyAsync(p => p.Id == dto.PriestProfileId);
            if (!exists) return BadRequest("Invalid PriestProfileId.");

            // Optional: prevent overlapping availability for same priest/date
            var overlap = await _db.Availabilities.AnyAsync(a =>
                a.PriestId == dto.PriestProfileId &&
                a.Date == dto.Date &&
                !(dto.EndTime <= a.StartTime || dto.StartTime >= a.EndTime));

            if (overlap) return Conflict("Overlapping availability exists.");

            var slot = new Availability
            {
                PriestId = dto.PriestProfileId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsAvailable = dto.IsAvailable
            };
            _db.Availabilities.Add(slot);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { priestProfileId = dto.PriestProfileId }, slot);
        }

        [Authorize(Roles = "Priest,Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var slot = await _db.Availabilities.FindAsync(id);
            if (slot is null) return NotFound();
            _db.Availabilities.Remove(slot);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

