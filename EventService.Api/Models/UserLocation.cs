using System.ComponentModel.DataAnnotations;

namespace EventService.Models
{
    public class UserLocation
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}