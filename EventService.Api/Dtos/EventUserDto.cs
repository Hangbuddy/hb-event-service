using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventUserDto
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}