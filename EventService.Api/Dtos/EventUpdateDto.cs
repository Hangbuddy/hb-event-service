using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace EventService.Dtos
{
    public class EventUpdateDto
    {
        [Key]
        [Required]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool PermissionRequired { get; set; }
        public bool IsActive { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}