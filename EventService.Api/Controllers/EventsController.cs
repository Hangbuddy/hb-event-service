using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using EventService.Data;
using EventService.Dtos;
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

        [HttpPost]
        public ActionResult<EventReadDto> CreateEvent(EventCreateDto eventCreateDto)
        {
            var eventModel = _mapper.Map<Event>(eventCreateDto);
            eventModel.OwnerId = User.FindFirst("Id")?.Value;
            _repository.CreateEvent(eventModel);
            _repository.SaveChanges();

            var eventReadDto = _mapper.Map<EventReadDto>(eventModel);

            return CreatedAtRoute(nameof(GetEvent), new { eventId = eventReadDto.Id }, eventReadDto);
        }

        [HttpPut]
        public ActionResult<EventReadDto> UpdateEvent(EventUpdateDto eventUpdateDto)
        {
            var eventModel = _mapper.Map<Event>(eventUpdateDto);
            eventModel.OwnerId = User.FindFirst("Id")?.Value;
            _repository.UpdateEvent(eventModel);
            _repository.SaveChanges();
            return CreatedAtRoute(nameof(GetEvent), new { eventId = eventUpdateDto.Id }, eventUpdateDto);
        }

        [HttpPost("{eventId}/register")]
        public ActionResult RegisterToEvent(string eventId)
        {
            var userId = User.FindFirst("Id")?.Value;
            var eventUserDto = new EventUserDto() { EventId = eventId, UserId = userId };
            var eventUserModel = _mapper.Map<EventUser>(eventUserDto);
            _repository.RegisterToEvent(eventUserModel);
            _repository.SaveChanges();
            return Ok();
        }

        [HttpPost("{eventId}/deregister")]
        public ActionResult DeRegisterFromEvent(string eventId)
        {
            var userId = User.FindFirst("Id")?.Value;
            var eventUserDto = new EventUserDto() { EventId = eventId, UserId = userId };
            var eventUserModel = _mapper.Map<EventUser>(eventUserDto);
            _repository.DeRegisterFromEvent(eventUserModel);
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
            var eventUserModel = _mapper.Map<EventUser>(eventUserUpdateDto);
            _repository.UpdateEventUser(eventUserModel);
            _repository.SaveChanges();
            return Ok();
        }

        [HttpGet("nearby-events", Name = "GetNearbyEvents")]
        public ActionResult<List<EventReadDto>> GetNearbyEvents(double latitude, double longitude, double range)
        {
            var events = _repository.GetNearbyEvents(latitude, longitude, range);
            if (events != null && events.Count > 0)
            {
                return Ok(_mapper.Map<List<EventReadDto>>(events));
            }
            return NotFound();
        }
    }
}