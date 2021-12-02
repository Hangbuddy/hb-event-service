using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos.Requests
{
    public class AreaDto
    {
        [Required]
        public UserLocationDto NorthEastLocation { get; set; }
        [Required]
        public UserLocationDto SouthWestLocation { get; set; }
    }
}