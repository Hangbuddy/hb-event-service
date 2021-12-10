using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using EventService.Data;
using EventService.Dtos.Requests;
using EventService.Dtos.Responses;
using EventService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

namespace EventService.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepo _repository;
        private readonly IMapper _mapper;

        public EventsController(
            IEventRepo repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("my-events", Name = "GetMyEvents")]
        public ActionResult<EventReadDto> GetMyEvents()
        {
            var userId = User.FindFirst("Id")?.Value;
            return Ok(_mapper.Map<List<EventReadDto>>(_repository.GetMyEvents(userId)));
        }

        [HttpGet("{eventId}", Name = "GetEvent")]
        public ActionResult<EventReadDto> GetEvent(string eventId)
        {
            var eventItem = _repository.GetEvent(eventId);
            if (eventItem != null)
            {
                return Ok(_mapper.Map<EventReadDto>(eventItem));
            }
            return NotFound();
        }

        [HttpPost(Name = "CreateEvent")]
        public ActionResult<EventReadDto> CreateEvent(EventCreateDto eventCreateDto)
        {
            var eventModel = _mapper.Map<Event>(eventCreateDto);
            // If user didn't set a start time then it's a default 12 hour event starting from now.
            if (eventCreateDto.StartTime == default)
                eventModel.StartTime = DateTime.Now;
            if (eventCreateDto.EndTime == default)
                eventModel.EndTime = eventModel.StartTime.AddHours(12);
            eventModel.OwnerId = User.FindFirst("Id")?.Value;
            _repository.CreateEvent(eventModel);
            _repository.SaveChanges();

            var eventReadDto = _mapper.Map<EventReadDto>(eventModel);

            return CreatedAtRoute(nameof(GetEvent), new { eventId = eventReadDto.Id }, eventReadDto);
        }

        [HttpPut(Name = "UpdateEvent")]
        public ActionResult<EventReadDto> UpdateEvent(EventUpdateDto eventUpdateDto)
        {
            var eventModel = _mapper.Map<Event>(eventUpdateDto);
            eventModel.OwnerId = User.FindFirst("Id")?.Value;
            _repository.UpdateEvent(eventModel);
            _repository.SaveChanges();
            return CreatedAtRoute(nameof(GetEvent), new { eventId = eventUpdateDto.Id }, eventUpdateDto);
        }

        [HttpPut("{eventId}/register", Name = "Register")]
        public ActionResult RegisterToEvent(string eventId)
        {
            var userId = User.FindFirst("Id")?.Value;
            var _event = _repository.GetEvent(eventId);
            var eventUser = new EventUser() { Event = _event, UserId = userId, Approved = false };
            _repository.RegisterToEvent(eventUser);
            _repository.SaveChanges();
            return Ok();
        }

        [HttpPut("{eventId}/deregister", Name = "DeRegister")]
        public ActionResult DeRegisterFromEvent(string eventId)
        {
            var userId = User.FindFirst("Id")?.Value;
            var _event = _repository.GetEvent(eventId);
            var eventUser = new EventUser() { Event = _event, UserId = userId, Approved = false };
            _repository.DeRegisterFromEvent(eventUser);
            _repository.SaveChanges();
            return Ok();
        }

        [HttpDelete("{eventId}/Users/{userId}", Name = "KickUser")]
        public ActionResult KickUser(string eventId, string userId)
        {
            var _event = _repository.GetEvent(eventId);
            var eventUser = new EventUser() { Event = _event, UserId = userId, Approved = false };
            _repository.DeRegisterFromEvent(eventUser);
            _repository.SaveChanges();
            return Ok();
        }

        [HttpGet("{eventId}/waiting-list", Name = "GetWaitingList")]
        public ActionResult<List<EventUserReadDto>> GetWaitingList(string eventId)
        {
            var eventUsers = _repository.GetWaitingList(eventId);
            if (eventUsers != null && eventUsers.Count > 0)
            {
                return Ok(_mapper.Map<List<EventUserReadDto>>(eventUsers));
            }
            return NotFound();
        }

        [HttpGet("{eventId}/approved-list", Name = "GetApprovedList")]
        public ActionResult<List<EventUserReadDto>> GetApprovedList(string eventId)
        {
            var eventUsers = _repository.GetApprovedList(eventId);
            if (eventUsers != null && eventUsers.Count > 0)
            {
                return Ok(_mapper.Map<List<EventUserReadDto>>(eventUsers));
            }
            return NotFound();
        }

        [HttpPost("update-event-user")]
        public ActionResult UpdateEventUser(EventUserUpdateDto eventUserUpdateDto)
        {
            _repository.UpdateEventUser(_mapper.Map<EventUser>(eventUserUpdateDto));
            _repository.SaveChanges();
            return Ok();
        }

        [HttpPost("nearby-events", Name = "GetNearbyEvents")]
        public ActionResult<List<EventReadDto>> GetNearbyEvents(UserLocationDto userLocationDto, double range)
        {
            var events = _repository.GetNearbyEvents(_mapper.Map<UserLocation>(userLocationDto), range);
            return Ok(_mapper.Map<List<EventReadDto>>(events));
        }

        [HttpPost("events-in-area", Name = "GetEventsInArea")]
        public ActionResult<List<EventReadDto>> GetEventsInArea(AreaDto areaDto)
        {
            var events = _repository.GetEventsInArea(_mapper.Map<Area>(areaDto));
            return Ok(_mapper.Map<List<EventReadDto>>(events));
        }

        [HttpGet("types", Name = "GetEventTypes")]
        public ActionResult<List<EventTypeDto>> GetEventTypes()
        {
            return Ok(EventTypeDto.EventTypes);
        }
    }
}