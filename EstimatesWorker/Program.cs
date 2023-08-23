using Shared;

const string eventHub = "estimates-domain";

while (true)
{
    var e = new Event()
    {
        Id = Guid.NewGuid(),
        DataVersion = "1.0",
        EventTime = DateTime.UtcNow,
        Type = EventType.EstimatesSigned,
        Data = Guid.NewGuid(),
    };
    ProducerConsumerFactory.Producer(Keys.connectionStringEH,eventHub,"ESTIMATES",e);

    Task.Delay(5000).Wait();
}
