using System;

namespace EventService.Dtos
{
    public class EventReadDto
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }
        public bool IsActive { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}