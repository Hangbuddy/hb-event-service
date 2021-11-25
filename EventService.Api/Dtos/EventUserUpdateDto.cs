using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventUserUpdateDto
    {
        [Required]
        public string EventId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool Approved { get; set; }
    }
}