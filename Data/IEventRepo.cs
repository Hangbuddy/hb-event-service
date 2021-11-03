using System.Collections.Generic;
using EventService.Models;

namespace EventService.Data
{
    public interface IEventRepo
    {
        bool SaveChanges();
        Event GetEvent(int eventId);
        void CreateEvent(Event _event);
        void UpdateEvent(Event _event);
        void RegisterToEvent(EventUser _event);
        void DeRegisterFromEvent(EventUser _event);

        // TODO:
        // List<EventUser> GetWaitingList(int eventId)
        // List<EventUser> GetApprovedList(int eventId)
    }
}