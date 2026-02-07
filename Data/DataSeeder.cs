using BarberShopReservation.Data;
using BarberShopReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberShopReservation.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Services.AnyAsync())
                return;

            var services = new List<Service>
            {
                new Service
                {
                    Name = "Strzyżenie męskie",
                    Description = "Profesjonalne strzyżenie włosów dla mężczyzn",
                    Price = 30.00m,
                    DurationMinutes = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Golenie",
                    Description = "Klasyczne golenie maszynką i żyletką",
                    Price = 25.00m,
                    DurationMinutes = 20,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Strzyżenie + Golenie",
                    Description = "Pakiet strzyżenie i golenie",
                    Price = 50.00m,
                    DurationMinutes = 45,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Stylizacja brody",
                    Description = "Profesjonalna stylizacja i pielęgnacja brody",
                    Price = 20.00m,
                    DurationMinutes = 15,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Service
                {
                    Name = "Farbowanie włosów",
                    Description = "Farbowanie włosów profesjonalnymi produktami",
                    Price = 40.00m,
                    DurationMinutes = 40,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Services.AddRangeAsync(services);
            await context.SaveChangesAsync();

            var timeSlots = new List<TimeSlot>();
            var baseDate = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

            foreach (var service in services)
            {
                for (int day = 0; day < 14; day++)
                {
                    for (int hour = 9; hour < 18; hour++)
                    {
                        var startTime = baseDate.AddDays(day).AddHours(hour - 9);
                        var endTime = startTime.AddMinutes(service.DurationMinutes);

                        if (startTime.DayOfWeek != DayOfWeek.Sunday)
                        {
                            timeSlots.Add(new TimeSlot
                            {
                                ServiceId = service.Id,
                                StartTime = startTime,
                                EndTime = endTime,
                                IsAvailable = true
                            });
                        }
                    }
                }
            }

            await context.TimeSlots.AddRangeAsync(timeSlots);
            await context.SaveChangesAsync();
        }
    }
}
