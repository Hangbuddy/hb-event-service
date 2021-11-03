using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using EventService.Data;
using EventService.Dtos;
using EventService.Models;

namespace EventService.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("get-event/{id}", Name = "GetEvent")]
        public ActionResult<EventReadDto> GetEvent(int eventId)
        {
            var eventItem = _repository.GetEvent(eventId);
            if (eventItem != null)
            {
                return Ok(_mapper.Map<EventReadDto>(eventItem));
            }
            return NotFound();
        }

        [HttpPost("create-event")]
        public ActionResult<EventReadDto> CreateEvent(EventCreateDto eventCreateDto)
        {
            var eventModel = _mapper.Map<Event>(eventCreateDto);
            _repository.CreateEvent(eventModel);
            _repository.SaveChanges();

            var eventReadDto = _mapper.Map<EventReadDto>(eventModel);

            return CreatedAtRoute(nameof(GetEvent), new { id = eventReadDto.Id }, eventReadDto);
        }

        [HttpPost("update-event")]
        public ActionResult<EventReadDto> UpdateEvent(EventUpdateDto eventUpdateDto)
        {
            var eventModel = _mapper.Map<Event>(eventUpdateDto);
            _repository.UpdateEvent(eventModel);
            _repository.SaveChanges();
            return CreatedAtRoute(nameof(GetEvent), new { id = eventUpdateDto.Id }, eventUpdateDto);
        }
        

        [HttpPost("register-to-event")]
        public ActionResult<EventReadDto> RegisterToEvent(EventUserDto eventUserDto)
        {
            var eventUserModel = _mapper.Map<EventUser>(eventUserDto);
            _repository.RegisterToEvent(eventUserModel);
            _repository.SaveChanges();
            return Ok();
        }

        

        [HttpPost("deregister-from-event")]
        public ActionResult<EventReadDto> DeRegisterFromEvent(EventUserDto eventUserDto)
        {
            var eventUserModel = _mapper.Map<EventUser>(eventUserDto);
            _repository.DeRegisterFromEvent(eventUserModel);
            _repository.SaveChanges();
            return Ok();
        }
    }
}