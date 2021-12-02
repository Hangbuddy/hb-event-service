using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using EventService.Data;
using EventService.Dtos.Requests;
using EventService.Dtos.Responses;
using EventService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
            var eventUser = new EventUser() { EventId = eventId, UserId = userId, Approved = false };
            _repository.RegisterToEvent(eventUser);
            _repository.SaveChanges();
            return Ok();
        }

        [HttpPut("{eventId}/deregister", Name = "Deregister")]
        public ActionResult DeRegisterFromEvent(string eventId)
        {
            var userId = User.FindFirst("Id")?.Value;
            var eventUser = new EventUser() { EventId = eventId, UserId = userId, Approved = false };
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

    }
}