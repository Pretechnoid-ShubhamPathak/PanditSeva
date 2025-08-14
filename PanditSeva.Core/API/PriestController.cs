// Controllers/PriestProfilesController.cs
using Data.Models;
using Data.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PanditSeva.Core.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriestController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PriestController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var priests = await _db.PriestProfiles
                .Include(p => p.User)
                .Include(p => p.PriestServices).ThenInclude(ps => ps.Service)
                .Select(p => new {
                    p.Id,
                    Name = p.User.Name,
                    Email = p.User.Email,
                    p.ContactNumber,
                    p.Bio,
                    Services = p.PriestServices.Select(ps => ps.Service!.Name)
                })
                .ToListAsync();

            return Ok(priests);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _db.PriestProfiles
                .Include(x => x.User)
                .Include(x => x.PriestServices).ThenInclude(ps => ps.Service)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p is null) return NotFound();
            return Ok(new
            {
                p.Id,
                Name = p.User.Name,
                Email = p.User.Email,
                p.ContactNumber,
                p.Bio,
                Services = p.PriestServices.Select(ps => new { ps.ServiceId, ps.Service!.Name })
            });
        }

        // Update profile (Priest or Admin)
        [Authorize(Roles = "Priest,Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PriestProfile update)
        {
            var p = await _db.PriestProfiles.FindAsync(id);
            if (p is null) return NotFound();

            p.Bio = update.Bio;
            p.ContactNumber = update.ContactNumber;
            await _db.SaveChangesAsync();
            return Ok(p);
        }

        // Set offered services (replace all)
        [Authorize(Roles = "Priest,Admin")]
        [HttpPut("{id:int}/services")]
        public async Task<IActionResult> SetServices(int id, [FromBody] IEnumerable<int> serviceIds)
        {
            var p = await _db.PriestProfiles.Include(x => x.PriestServices).FirstOrDefaultAsync(x => x.Id == id);
            if (p is null) return NotFound();

            _db.PriestServices.RemoveRange(p.PriestServices);
            await _db.SaveChangesAsync();

            var validIds = await _db.Services.Where(s => serviceIds.Contains(s.Id)).Select(s => s.Id).ToListAsync();
            var links = validIds.Select(sid => new PriestService { PriestProfileId = id, ServiceId = sid });
            await _db.PriestServices.AddRangeAsync(links);
            await _db.SaveChangesAsync();

            return Ok(new { PriestProfileId = id, ServiceIds = validIds });
        }
    }

}