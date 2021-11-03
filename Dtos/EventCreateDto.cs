using System;
using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventCreateDto
    {
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
    }
}