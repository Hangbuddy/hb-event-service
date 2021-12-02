using System;
using System.ComponentModel.DataAnnotations;
using EventService.Dtos.Enums;
using NetTopologySuite.Geometries;

namespace EventService.Models
{
    public class Event
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string OwnerId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public EventType EventType { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }
        public bool IsActive { get; set; }
        public Point Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}