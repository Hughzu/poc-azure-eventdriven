using Shared;

const string connectionString = "Endpoint=sb://poceventhubns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Nl86kAe9uXau5CeTvENKckMBTxofTmJ5k+AEhB+n1/w=";
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
    ProducerConsumerFactory.Producer(connectionString,eventHub,"ESTIMATES",e);

    Task.Delay(5000).Wait();
}
