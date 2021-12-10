using System.Collections.Generic;
using EventService.Models;

namespace EventService.Data
{
    public interface IEventRepo
    {
        bool SaveChanges();
        List<Event> GetMyEvents(string userId);
        Event GetEvent(string eventId);
        bool CreateEvent(Event _event);
        void UpdateEvent(Event _event);
        void RegisterToEvent(EventUser _event);
        void DeRegisterFromEvent(EventUser _event);
        List<EventUser> GetWaitingList(string eventId);
        List<EventUser> GetApprovedList(string eventId);
        void UpdateEventUser(EventUser _event);
        List<Event> GetNearbyEvents(UserLocation location, double range);
        List<Event> GetEventsInArea(Area area);

    
    }
}