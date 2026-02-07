using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BarberShopReservation.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa usługi jest wymagana")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Czas trwania usługi jest wymagany")]
        [Range(5, 480, ErrorMessage = "Czas trwania musi być między 5 a 480 minut")]
        public int DurationMinutes { get; set; } = 30;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
