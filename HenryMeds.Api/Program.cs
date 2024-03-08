
using HenryMeds.Api.Data;
using HenryMeds.Api.Data.Models;
using HenryMeds.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HenryMeds.Api
{
    public class Program
    {
        private static ILogger<Program>? logger;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("HenryInMemoryDb"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            logger = app.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Starting the Henry Meds Api...");

            // Create a scope to get the service provider.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContextOptions = services.GetRequiredService<DbContextOptions<AppDbContext>>();
                    DataSeeder.Initialize(services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // ** MINIMAL API ENDPOINTS Below ** 

            // 1) Endpoint to allow providers to submit their availability.
            app.MapPost("/providers/availability", async (AppDbContext db, AppointmentSlot slot) =>
            {
                var (isValid, errors) = ValidationHelper.ValidateModel(slot);

                if (!isValid)
                {
                    return Results.BadRequest(errors);
                }

                // Check if a similar slot already exists.
                var slotExists = await db.AppointmentSlots.AnyAsync(slotData =>
                    slotData.ProviderId == slot.ProviderId &&
                    slotData.StartTime == slot.StartTime &&
                    slotData.Duration == slot.Duration);

                if (slotExists)
                {
                    // Return a conflict response indicating the slot already exists.
                    return Results.Conflict("An identical appointment slot already exists.");
                }

                db.AppointmentSlots.Add(slot);
                await db.SaveChangesAsync();
                return Results.Created($"/providers/availability/{slot.Id}", slot);
            })
            .WithOpenApi(); 

            // 2) Endpoint for clients to view available appointment slots.
            app.MapGet("/appointmentSlots/available", async (AppDbContext db) =>
            {
                var now = DateTime.UtcNow;
                var availableSlots = await db.AppointmentSlots
                    .Where(s => !s.IsReserved && s.StartTime > now.AddHours(24))
                    .ToListAsync();

                return Results.Ok(availableSlots);
            })
            .WithOpenApi();

            // 3) Endpoint to allow clients to reserve an appointment slot.
            app.MapPost("/reservations", async (AppDbContext db, Reservation reservation) =>
            {
                // Set the reservation time to the current time. This will expire after 30 mins if not confirmed. 
                reservation.ReservationTime = DateTime.UtcNow;

                var (isValid, errors) = ValidationHelper.ValidateModel(reservation);

                if (!isValid)
                {
                    return Results.BadRequest(errors);
                }

                var slot = await db.AppointmentSlots.FindAsync(reservation.AppointmentSlotId);
                if (slot == null || slot.IsReserved || slot.StartTime <= DateTime.UtcNow.AddHours(24))
                {
                    return Results.BadRequest("Slot is either reserved, does not exist, or is within the next 24 hours.");
                }
                
                slot.IsReserved = true; // Mark slot as reserved.
                db.Reservations.Add(reservation);
                await db.SaveChangesAsync();

                return Results.Created($"/reservations/{reservation.Id}", reservation);
            })
            .WithOpenApi();

            // 4) Endpoint to confirm a reservation.
            app.MapPut("/reservations/confirm/{id}", async (AppDbContext db, int id) =>
            {
                var reservation = await db.Reservations.FindAsync(id);
                if (reservation == null || reservation.IsConfirmed)
                {
                    return Results.NotFound("Reservation not found or already confirmed.");
                } else if (reservation.ReservationTime.AddMinutes(30) < DateTime.UtcNow)
                {
                    return Results.BadRequest("Reservation has expired.");
                }

                reservation.IsConfirmed = true;
                await db.SaveChangesAsync();
                return Results.Ok(reservation);
            })
            .WithOpenApi();

            app.Run();
        }
    }
}
