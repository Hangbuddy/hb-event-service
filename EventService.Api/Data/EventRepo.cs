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
            return _context.SaveChanges() >= 0;
        }

        public List<Event> GetMyEvents(string userId)
        {
            return _context.EventUsers
                            .Where(eu => eu.UserId == userId && eu.Approved)
                            .Select(eu => eu.Event).ToList();
        }

        public Event GetEvent(string eventId)
        {
            return _context.Events.FirstOrDefault(e => e.Id == new Guid(eventId));
        }

        public bool CreateEvent(Event _event)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (_event == null)
                    throw new ArgumentNullException(nameof(_event));
                _context.Events.Add(_event);
                RegisterToEvent(new EventUser() { Event = _event, UserId = _event.OwnerId, Approved = true });
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
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
            return _context.EventUsers.Where(e => e.Event.Id.ToString() == eventId && !e.Approved).ToList();
        }

        public List<EventUser> GetApprovedList(string eventId)
        {
            return _context.EventUsers.Where(e => e.Event.Id.ToString() == eventId && e.Approved).ToList();
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

        public List<Event> GetNearbyEvents(UserLocation userLocation, double range)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var location = geometryFactory.CreatePoint(new Coordinate(userLocation.Longitude, userLocation.Latitude));

            return _context.Events.Where(e => e.Location.Distance(location) < range).ToList();
        }


        public List<Event> GetEventsInArea(Area area)
        {
            Coordinate northEastCoordinate = new(area.NorthEastLocation.Longitude, area.NorthEastLocation.Latitude);
            Coordinate southWestCoordinate = new(area.SouthWestLocation.Longitude, area.SouthWestLocation.Latitude);
            Coordinate northWestCoordinate = new(area.SouthWestLocation.Longitude, area.NorthEastLocation.Latitude);
            Coordinate southEastCoordinate = new(area.NorthEastLocation.Longitude, area.SouthWestLocation.Latitude);

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var coordinatesArray = new Coordinate[] { northEastCoordinate, northWestCoordinate, southWestCoordinate, southEastCoordinate, northEastCoordinate };
            var polygon = geometryFactory.CreatePolygon(coordinatesArray);
            return _context.Events.Where(e => polygon.Contains(e.Location)).ToList();
        }
    }
}