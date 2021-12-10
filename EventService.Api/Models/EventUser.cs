using System.ComponentModel.DataAnnotations;

namespace EventService.Models
{
    public class EventUser
    {
        [Required]
        public Event Event { get; set; }
        [Required]
        public string UserId { get; set; }
        public bool Approved { get; set; }
    }
}