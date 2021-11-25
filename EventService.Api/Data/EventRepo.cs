using System;
using System.Collections.Generic;
using System.Linq;
using EventService.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;

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

        public Event GetEvent(string eventId)
        {
            return _context.Events.FirstOrDefault(p => p.Id == new Guid(eventId));
        }

        public void CreateEvent(Event _event)
        {
            if (_event == null)
                throw new ArgumentNullException(nameof(_event));
            _context.Events.Add(_event);
        }

        public void UpdateEvent(Event _event)
        {
            if (_event == null)
                throw new ArgumentNullException(nameof(_event));
            _context.Events.Update(_event);
        }

        public void RegisterToEvent(EventUser eventUser)
        {
            if (eventUser == null)
                throw new ArgumentNullException(nameof(eventUser));
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

        public List<EventUser> GetWaitingList(string eventId)
        {
            return _context.EventUsers.Where(e => e.EventId == eventId && !e.Approved).ToList();
        }

        public List<EventUser> GetApprovedList(string eventId)
        {
            return _context.EventUsers.Where(e => e.EventId == eventId && e.Approved).ToList();
        }

        public void UpdateEventUser(EventUser eventUser)
        {
            if (eventUser == null)
                throw new ArgumentNullException(nameof(eventUser));
            // In case of rejection remove the request completely.
            if (!eventUser.Approved)
                DeRegisterFromEvent(eventUser);
            else
                _context.EventUsers.Update(eventUser);
        }
        public List<Event> GetNearbyEvents(double lattidute, double longtidute, double range)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var location = geometryFactory.CreatePoint(new Coordinate(longtidute, lattidute));

            return _context.Events.Where(e => e.Location.Distance(location) < range).ToList();
        }
    }
}