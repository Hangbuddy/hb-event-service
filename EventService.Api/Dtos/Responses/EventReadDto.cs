using System;
using System.Collections.Generic;

namespace EventService.Dtos.Responses
{
    public class EventReadDto
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public EventTypeDto EventType { get; set; }
        public ICollection<EventUserReadDto> EventUsers { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int BuddiesCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}