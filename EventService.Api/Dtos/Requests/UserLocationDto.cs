using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos.Requests
{
    public class UserLocationDto
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}