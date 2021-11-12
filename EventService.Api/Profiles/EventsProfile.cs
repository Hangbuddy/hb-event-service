using System;
using AutoMapper;
using EventService.Dtos;
using EventService.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EventService.Profiles
{
    public class EventsProfile : Profile
    {
        public EventsProfile()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            // Source -> Target
            CreateMap<EventReadDto, Event>()
                .ForMember(dest => dest.Location, opt =>
                opt.MapFrom(src => geometryFactory.CreatePoint(new Coordinate(src.Longitude, src.Latitude))));
            CreateMap<Event, EventReadDto>()
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y));
            CreateMap<EventCreateDto, Event>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Location, opt =>
                opt.MapFrom(src => geometryFactory.CreatePoint(new Coordinate(src.Longitude, src.Latitude))));
            CreateMap<EventUpdateDto, Event>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Location, opt =>
                opt.MapFrom(src => geometryFactory.CreatePoint(new Coordinate(src.Longitude, src.Latitude))));
            CreateMap<EventUserDto, EventUser>();
            CreateMap<EventUser, EventUserDto>();
            CreateMap<EventUserReadDto, EventUser>();
            CreateMap<EventUser, EventUserReadDto>();
            CreateMap<EventUserUpdateDto, EventUser>();
            CreateMap<EventUser, EventUserUpdateDto>();
        }
    }
}