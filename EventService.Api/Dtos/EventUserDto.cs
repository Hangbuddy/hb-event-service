using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventUserDto
    {
        [Required]
        public string EventId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}