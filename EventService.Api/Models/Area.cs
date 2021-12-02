using System.ComponentModel.DataAnnotations;

namespace EventService.Models
{
    public class Area
    {
        [Required]
        public UserLocation NorthEastLocation { get; set; }
        [Required]
        public UserLocation SouthWestLocation { get; set; }
    }
}