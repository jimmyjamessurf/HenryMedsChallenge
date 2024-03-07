using HenryMeds.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HenryMeds.Api.Data
{
    public class DataSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                // Skip seeding if data already exists.
                if (dbContext.Providers.Any())
                {
                    return; // DB has been seeded
                }

                SeedData(dbContext);
            }
        }

        private static void SeedData(AppDbContext dbContext)
        {
            // Create sample providers
            var providers = new[]
            {
                new Provider { Name = "Dr. Jekyll" },
                new Provider { Name = "Dr. Who" }
    
            };

            dbContext.Providers.AddRange(providers);
            dbContext.SaveChanges();

            // Create sample appointment slots.
            var appointmentSlots = new[]
            {
                new AppointmentSlot { ProviderId = providers[0].Id, StartTime = DateTime.UtcNow.AddDays(3).Date.AddHours(10), Duration = TimeSpan.FromMinutes(15), IsReserved = false },
                new AppointmentSlot { ProviderId = providers[1].Id, StartTime = DateTime.UtcNow.AddDays(3).Date.AddHours(8), Duration = TimeSpan.FromMinutes(15), IsReserved = false },
            
            };

            dbContext.AppointmentSlots.AddRange(appointmentSlots);
            dbContext.SaveChanges();

            // Create sample clients
            var clients = new[]
            {
                new Client { Name = "Alice Wonderland", Email = "alice@example.com" },
                new Client { Name = "Bob Builder", Email = "bob@example.com" }
            };

            dbContext.Clients.AddRange(clients);
            dbContext.SaveChanges();


        }
    }
}
