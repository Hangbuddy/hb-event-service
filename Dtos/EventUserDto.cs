
using System;
using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventUserDto
    {
        [Required]
        public int EventId { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool Approved { get; set; }
    }
}