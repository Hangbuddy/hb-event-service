using System.Collections.Generic;
using System.Linq;

namespace EventService.Dtos.Responses
{
    public sealed class EventTypeDto
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Ascii;

        private EventTypeDto(int id, string name, string ascii) { Id = id; Name = name; Ascii = ascii; }

        public static readonly EventTypeDto Other = new(1, "Other", "U+1F937");
        public static readonly EventTypeDto Walk = new(2, "Walk", "U+1F6B6");
        public static readonly EventTypeDto Coffee = new(3, "Coffee", "U+2615");
        public static readonly EventTypeDto Beer = new(4, "Beer", "U+1F37B");
        public static readonly EventTypeDto Cocktail = new(5, "Cocktail", "U+1F378");
        public static readonly EventTypeDto HouseParty = new(6, "HouseParty", "U+1F389");
        public static readonly EventTypeDto Food = new(7, "Food", "U+1F355");
        public static readonly EventTypeDto Study = new(8, "Study", "U+1F4DA");
        public static readonly EventTypeDto Game = new(9, "Game", "U+1F3AE");

        public static readonly List<EventTypeDto> EventTypes = new() { Other, Walk, Coffee, Beer, Cocktail, HouseParty, Food, Study, Game };
        public static EventTypeDto FromId(int id) { return EventTypes.FirstOrDefault(eventType => eventType.Id == id) ?? Other; }
    }
}