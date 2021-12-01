using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }

    }
}