using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Data.Models.Enums;


namespace Data.Repos
{
        public static class DbSeeder
        {
            public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
            {
                await context.Database.MigrateAsync();

                // Roles
                foreach (var role in Enum.GetNames(typeof(UserRole)))
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole<int>(role));
                }

                // Services
                if (!context.Services.Any())
                {
                    context.Services.AddRange(
                        new Services { Name = "Housewarming (Griha Pravesh)", Description = "Standard puja", BasePrice = 150 },
                        new Services { Name = "Satyanarayan Katha", Description = "Puja & Katha", BasePrice = 120 },
                        new Services { Name = "Wedding (Vivah)", Description = "Complete ceremony", BasePrice = 300 }
                        );
                await context.SaveChangesAsync();
                }

                // Default Priest
                if (!userManager.Users.Any())
                {
                    var priest = new ApplicationUser
                    {
                        UserName = "priest1",
                        Email = "priest1@example.com",
                        Name = "Pandit Ram",
                        Phone = "9999999999",
                        Role = UserRole.Priest,
                        EmailConfirmed = true,
                        PasswordHash = "test@123",
                    };

                    await userManager.CreateAsync(priest, "Priest@123");
                    await userManager.AddToRoleAsync(priest, UserRole.Priest.ToString());

                    var profile = new PriestProfile
                    {
                        UserId = priest.Id,
                        Bio = "Expert in Vedic rituals",
                        ContactNumber = "9999999999"
                    };

                context.PriestProfiles.Add(profile);
                await context.SaveChangesAsync();

                var puja = await context.Services.Where(s => s.Name.Contains("Housewarming")).FirstAsync();
                var wedding = await context.Services.Where(s => s.Name.Contains("Wedding")).FirstAsync();
                context.PriestServices.AddRange(
                    new PriestService { PriestProfileId = profile.Id, ServiceId = puja.Id, Service = puja, PriestProfile = profile },
                    new PriestService { PriestProfileId = profile.Id, ServiceId = wedding.Id, Service = wedding, PriestProfile = profile }
                );

                // Availability
                context.Availabilities.AddRange(
                    new Availability { PriestId= profile.Id, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), StartTime = new(07,30), EndTime = new(18,00), IsAvailable = true },
                    new Availability { PriestId = profile.Id, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)), StartTime = new(07, 30), EndTime = new(18, 00), IsAvailable = true }
                );

                await context.SaveChangesAsync();
                }
            }
        }
    }
