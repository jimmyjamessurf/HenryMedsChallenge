using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HenryMeds.Api.Data.Models
{
    public class AppointmentSlot
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProviderId must be greater than 0.")]
        public int ProviderId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15); // Default to 15 minutes

        [Required]
        public bool IsReserved { get; set; } = false;

        public Provider Provider { get; set; }
      
    }

}
