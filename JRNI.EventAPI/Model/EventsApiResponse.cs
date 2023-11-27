namespace JRNI.EventAPI.Model
{
    public class EventsApiResponse
    {
        public string Email { get; set; }
        public int Number_of_events { get; set; }
        public List<Event> Events { get; set; }
    }
}