using System.ComponentModel.DataAnnotations;

namespace HenryMeds.Api.Data.Models
{
    public class Client
    {
        public int Id { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

}
