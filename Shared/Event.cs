namespace Shared
{
    public class Event
    {
        public Guid Id { get; set; }
        public EventType Type { get; set; }
        public object? Data { get; set; }

        public DateTime EventTime { get; set; }
        public string? DataVersion { get; set; }

    }
}