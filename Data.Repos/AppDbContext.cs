using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Repos
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PriestProfile> PriestProfiles { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<PriestService> PriestServices { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Composite key for PriestService (many-to-many)
            builder.Entity<PriestService>()
                .HasKey(ps => new { ps.PriestProfileId, ps.ServiceId });

            // Relationships
            builder.Entity<PriestService>()
                .HasOne(ps => ps.PriestProfile)
                .WithMany(p => p.PriestServices)
                .HasForeignKey(ps => ps.PriestProfileId);

            builder.Entity<PriestService>()
                .HasOne(ps => ps.Service)
                .WithMany(s => s.PriestServices)
                .HasForeignKey(ps => ps.ServiceId);

            builder.Entity<PriestProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.PriestProfile!)
                .HasForeignKey<PriestProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Booking>()
                .HasIndex(bk => new { bk.PriestId, bk.Date });
        }
    }
}
