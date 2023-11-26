namespace JRNI.EventAPI.Model
{
    public class Event
    {
        public string Subject { get; set; }
        public string Status { get; set; }
        public DateTime Start_time { get; set; }
        public DateTime End_time { get; set; }
        public string Id { get; set; }
    }
}