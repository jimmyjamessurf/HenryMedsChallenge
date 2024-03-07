using System.ComponentModel.DataAnnotations;

namespace HenryMeds.Api.Data.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "AppointmentSlotId must be greater than 0.")]

        public int AppointmentSlotId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ClientId must be greater than 0.")]
        public int ClientId { get; set; } 

        [Required]
        public DateTime ReservationTime { get; set; }

        [Required]
        public bool IsConfirmed { get; set; } = false;

        public AppointmentSlot AppointmentSlot { get; set; }

        public Client Client { get; set; }
    }

}
