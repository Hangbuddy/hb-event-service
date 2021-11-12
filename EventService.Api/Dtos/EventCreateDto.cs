using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventCreateDto
    {
        [Required]
        public string OwnerId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}