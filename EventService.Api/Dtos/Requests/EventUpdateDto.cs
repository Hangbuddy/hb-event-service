using System;
using System.ComponentModel.DataAnnotations;
using EventService.Dtos.Responses;

namespace EventService.Dtos.Requests
{
    public class EventUpdateDto
    {
        [Key]
        [Required]
        public string Id { get; set; }
        public string Title { get; set; }
        public EventTypeDto EventType { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}