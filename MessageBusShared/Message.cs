namespace MessageBusShared
{
    public class Message
    {
        public Guid Id { get; set; }
        public MessageType Type { get; set; }
        public string? Data { get; set; }

        public DateTime MessageTime { get; set; }
        public string? DataVersion { get; set; }

    }
}