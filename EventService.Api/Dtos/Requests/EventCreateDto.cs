using System;
using System.ComponentModel.DataAnnotations;
using EventService.Dtos.Responses;

namespace EventService.Dtos.Requests
{
    public class EventCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public EventTypeDto EventType { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}