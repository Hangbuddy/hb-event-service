using System;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace EventService.Models
{
    public class Event
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }
        public bool IsActive { get; set; }
        public Point Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}