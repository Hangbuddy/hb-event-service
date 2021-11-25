using System.Collections.Generic;
using EventService.Models;

namespace EventService.Data
{
    public interface IEventRepo
    {
        bool SaveChanges();
        Event GetEvent(string eventId);
        void CreateEvent(Event _event);
        void UpdateEvent(Event _event);
        void RegisterToEvent(EventUser _event);
        void DeRegisterFromEvent(EventUser _event);
        List<EventUser> GetWaitingList(string eventId);
        List<EventUser> GetApprovedList(string eventId);
        void UpdateEventUser(EventUser _event);
        List<Event> GetNearbyEvents(double lattidute, double longtidute, double range);
    }
}