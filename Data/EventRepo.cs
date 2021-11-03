using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventService.Models;

namespace EventService.Data
{
    public class EventRepo : IEventRepo
    {
        private readonly AppDbContext _context;

        public EventRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public Event GetEvent(int eventId)
        {
            return _context.Events.FirstOrDefault(p => p.Id == eventId);
        }

        public void CreateEvent(Event _event)
        {
            if (_event == null)
            {
                throw new ArgumentNullException(nameof(_event));
            }
            _context.Events.Add(_event);
        }

        public void UpdateEvent(Event _event)
        {
            if (_event == null)
            {
                throw new ArgumentNullException(nameof(_event));
            }
            _context.Events.Update(_event);
        }

        public void RegisterToEvent(EventUser eventUser)
        {
            if (eventUser == null)
            {
                throw new ArgumentNullException(nameof(eventUser));
            }
            _context.EventUsers.Add(eventUser);
        }

        public void DeRegisterFromEvent(EventUser eventUser)
        {
            if (eventUser == null)
            {
                throw new ArgumentNullException(nameof(eventUser));
            }
            _context.EventUsers.Remove(eventUser);
        }
    }
}