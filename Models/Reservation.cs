using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShopReservation.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int TimeSlotId { get; set; }

        [Required(ErrorMessage = "Status rezerwacji jest wymagany")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Cancelled, Completed

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        [ForeignKey("TimeSlotId")]
        public TimeSlot? TimeSlot { get; set; }
    }
}
