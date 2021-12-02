using System.ComponentModel.DataAnnotations;
using EventService.Dtos.Enums;

namespace EventService.Dtos.Requests
{
    public class EventCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public EventType EventType { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public string Description { get; set; }

    }
}