
using System;
using System.ComponentModel.DataAnnotations;

namespace EventService.Models
{
    public class EventUser
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public int UserId { get; set; }

        public bool Approved { get; set; }
    }
}