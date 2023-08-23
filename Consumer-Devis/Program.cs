using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System.Collections.Concurrent;
using System.Text;
using Shared_Poc_1;

//const string consumerGroup = "devis-eh";
const string consumerGroup = "external-communication";

var storageClient = new BlobContainerClient(
    Keys.storageAccountEndPoint
    , "devis-container");
var processor = new EventProcessorClient(
    storageClient,
    EventHubConsumerClient.DefaultConsumerGroupName,
    Keys.storageAccountEndPoint,
    consumerGroup,
    new EventProcessorClientOptions
    {
        MaximumWaitTime = TimeSpan.FromMilliseconds(100)
    });

// checkpoint 
const int EventsBeforeCheckpoint = 5;
var partitionEventCount = new ConcurrentDictionary<string, int>();

processor.ProcessEventAsync += processEventHandler;
processor.ProcessErrorAsync += processErrorHandler;

Console.WriteLine("Start processing ...");
await processor.StartProcessingAsync();
//await Task.Delay(TimeSpan.FromSeconds(5));
//Console.WriteLine("Start processing ...");
//await processor.StopProcessingAsync();

Thread.Sleep(Timeout.Infinite);


async Task processEventHandler(ProcessEventArgs eventArgs)
{
    if (eventArgs.Data == null)
        return;

    Console.WriteLine($"\tPartition : {eventArgs.Partition.PartitionId} - Data: {Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray())}");

    string partition = eventArgs.Partition.PartitionId;
    int eventsSinceLastCheckpoint = partitionEventCount.AddOrUpdate(
        key: partition,
        addValue: 1,
        updateValueFactory: (_, currentCount) => currentCount + 1);

    if (eventsSinceLastCheckpoint >= EventsBeforeCheckpoint)
    {
        await eventArgs.UpdateCheckpointAsync();
        partitionEventCount[partition] = 0;
        Console.WriteLine($"\t\tCheckpoint made for partition '{partition}'");
    }

    //AppDomain.CurrentDomain.SendHeartbeatAsync(eventArgs.CancellationToken);
}

Task processErrorHandler(ProcessErrorEventArgs eventArgs)
{
    Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
    Console.WriteLine(eventArgs.Exception.Message);
    return Task.CompletedTask;
}