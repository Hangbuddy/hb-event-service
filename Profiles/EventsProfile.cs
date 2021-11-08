using System;
using AutoMapper;
using EventService.Dtos;
using EventService.Models;

namespace EventService.Profiles
{
    public class EventsProfile : Profile
    {
        public EventsProfile()
        {
            // Source -> Target
            CreateMap<EventReadDto, Event>();
            CreateMap<Event, EventReadDto>();
            CreateMap<EventCreateDto, Event>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<EventUpdateDto, Event>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<EventUserDto, EventUser>();
            CreateMap<EventUser, EventUserDto>();
            CreateMap<EventUserReadDto, EventUser>();
            CreateMap<EventUser, EventUserReadDto>();
            CreateMap<EventUserUpdateDto, EventUser>();
            CreateMap<EventUser, EventUserUpdateDto>();
        }
    }
}