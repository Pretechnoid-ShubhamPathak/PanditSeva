// Controllers/BookingsController.cs
using Data.Models.Enums;
using Data.Models;
using Data.Repos;
using Data.Standard.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PanditSeva.Core.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BookingController(AppDbContext db) => _db = db;

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _db.Bookings
                .Include(b => b.Service)
                .Include(b => b.PriestProfile)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking is null ? NotFound() : Ok(booking);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int? priestProfileId = null, [FromQuery] BookingStatus? status = null)
        {
            var q = _db.Bookings
                .Include(b => b.Service)
                .Include(b => b.PriestProfile).ThenInclude(p => p.User)
                .AsQueryable();

            if (priestProfileId is not null) q = q.Where(b => b.PriestId == priestProfileId);
            if (status is not null) q = q.Where(b => b.Status == status);

            var list = await q.OrderByDescending(b => b.Date).ThenBy(b => b.StartTime).ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            // Validate foreign keys
            var priest = await _db.PriestProfiles.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == dto.PriestProfileId);
            var serviceExists = await _db.Services.FirstOrDefaultAsync(s => s.Id == dto.ServiceId);
            var user = await _db.Users.FirstOrDefaultAsync(s => s.Id == dto.UserId);
            if (priest is null || serviceExists is null || user is null) return BadRequest("Invalid priest or service or user.");

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime.");

            // Ensure priest has availability that covers requested time (optional strict mode)
            var hasAvailability = await _db.Availabilities.AnyAsync(a =>
                a.PriestId == dto.PriestProfileId &&
                a.Date == dto.Date &&
                a.IsAvailable &&
                dto.StartTime >= a.StartTime && dto.EndTime <= a.EndTime);
            if (!hasAvailability)
                return Conflict("Requested time is not within available slots.");

            // Overlap check with existing bookings (except cancelled)
            var overlap = await _db.Bookings.AnyAsync(b =>
                b.PriestId == dto.PriestProfileId &&
                b.Date == dto.Date &&
                b.Status != BookingStatus.Cancelled &&
                !(dto.EndTime <= b.StartTime || dto.StartTime >= b.EndTime));

            if (overlap) return Conflict("Selected time overlaps an existing booking.");

            var booking = new Booking
            {
                UserId = dto.UserId,
                PriestId = dto.PriestProfileId,
                ServiceId = dto.ServiceId,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = BookingStatus.Pending,
                Service = serviceExists,
                User = user,
                PriestProfile = priest
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }

        // Priest/Admin updates booking status (e.g., Accepted/Cancelled/Completed)
        [Authorize(Roles = "Priest,Admin")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto dto)
        {
            var booking = await _db.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            booking.Status = dto.Status;
            await _db.SaveChangesAsync();
            return Ok(booking);
        }

        // Mark as Paid (e.g., after gateway webhook). Admin only here for simplicity.
        //[Authorize(Roles = "Admin")]
        //[HttpPut("{id:int}/paid")]
        //public async Task<IActionResult> MarkPaid(int id, [FromBody] decimal amount)
        //{
        //    var booking = await _db.Bookings.FindAsync(id);
        //    if (booking is null) return NotFound();

        //    booking.Status = BookingStatus.Paid;
        //    booking.AmountPaid = amount;
        //    await _db.SaveChangesAsync();
        //    return Ok(booking);
        //}
    }

}
