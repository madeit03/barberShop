using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarberShopReservation.Models;

namespace BarberShopReservation.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<TimeSlot> TimeSlots { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Price precision
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            // Service relationships
            modelBuilder.Entity<Service>()
                .HasMany(s => s.TimeSlots)
                .WithOne(t => t.Service)
                .HasForeignKey(t => t.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Service>()
                .HasMany(s => s.Reservations)
                .WithOne(r => r.Service)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // TimeSlot relationships
            modelBuilder.Entity<TimeSlot>()
                .HasOne(t => t.Reservation)
                .WithOne(r => r.TimeSlot)
                .HasForeignKey<Reservation>(r => r.TimeSlotId)
                .OnDelete(DeleteBehavior.NoAction);

            // Reservation relationships
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<TimeSlot>()
                .HasIndex(t => new { t.ServiceId, t.StartTime })
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.UserId, r.ServiceId });
        }
    }
}
