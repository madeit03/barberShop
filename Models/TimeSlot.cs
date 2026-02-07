using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShopReservation.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Data i czas są wymagane")]
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        public Reservation? Reservation { get; set; }
    }
}
