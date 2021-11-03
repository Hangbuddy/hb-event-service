using System;
using System.ComponentModel.DataAnnotations;

namespace EventService.Dtos
{
    public class EventUpdateDto
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }
        public bool IsActive { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }

    }
}