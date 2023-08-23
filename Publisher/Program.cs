using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Globalization;
using System.Text;


var producerClient = new EventHubProducerClient(
    "Endpoint=sb://poceventhubns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Nl86kAe9uXau5CeTvENKckMBTxofTmJ5k+AEhB+n1/w=",
    "devis-eh");

//async Task ExitHandler()
//{
//    Console.WriteLine("Shutting down");
//    await producerClient.DisposeAsync();
//}
//AppDomain.CurrentDomain.ProcessExit += (sender, args) => ExitHandler();


while (true)
{
    using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync())
    {
        for (int i = 1; i <= 2; i++)
        {
            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff.ffffff", CultureInfo.InvariantCulture)}"))))
            {
                throw new Exception($"Event too large for the batch and cannot be sent.");
            }
        }

        try
        {
            await producerClient.SendAsync(eventBatch);
            Console.WriteLine($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)} - A batch of {eventBatch?.Count} events has been published.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

       
    Task.Delay(5000).Wait();
}

