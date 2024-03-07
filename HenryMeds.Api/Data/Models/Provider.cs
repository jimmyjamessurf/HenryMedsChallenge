using System.ComponentModel.DataAnnotations;

namespace HenryMeds.Api.Data.Models
{
    public class Provider
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<AppointmentSlot> AppointmentSlots { get; set; } = new List<AppointmentSlot>();
    }

}
