using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Shared
{
    public static class ProducerConsumerFactory
    {
        public static async void Producer(string connectionString, string eventHub, string workerName, Event e)
        {
            var producerClient = new EventHubProducerClient(connectionString, eventHub);

            using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync())
            {
                try
                {
                    if (!eventBatch.TryAdd(new EventData(JsonSerializer.Serialize(e))))
                    {
                        throw new Exception($"Event too large for the batch and cannot be sent.");
                    }

                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine($"[{workerName}:{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}] - An event has been sent.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    await producerClient.DisposeAsync();
                }
            }
        }

        public static async void Consumer(string connectionStringSA, string container, string connectionStringEH, string eventHub, string workerName,
            Func<ProcessEventArgs, Task> processEventHandler, Func<ProcessErrorEventArgs, Task> processErrorHandler)
        {
            var storageClient = new BlobContainerClient(connectionStringSA, container);
            var processor = new EventProcessorClient(storageClient, EventHubConsumerClient.DefaultConsumerGroupName, connectionStringEH, eventHub);

            processor.ProcessEventAsync += processEventHandler;
            processor.ProcessErrorAsync += processErrorHandler;

            Console.WriteLine($"{workerName} - Start processing ...");
            await processor.StartProcessingAsync();
            Thread.Sleep(Timeout.Infinite);
        }

        public static Func<ProcessEventArgs,Task> ProcessEventHandlerTemplate(string workerName, Action? method = null, List<EventType>? interestingEvents = null)
        {
            return (ProcessEventArgs eventArgs) =>
            {
                if (eventArgs.Data?.Body == null) return Task.CompletedTask;

                Event? e = JsonSerializer.Deserialize<Event>(Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

                if (e == null) return Task.CompletedTask;
                if (interestingEvents != null && !interestingEvents.Contains(e.Type)) return Task.CompletedTask;

                Console.WriteLine($"[{workerName}-{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}] - event received [{e.Type}/{e.Id}]");

                method?.Invoke();

                return Task.CompletedTask;
            };
        }

        public static Task ProcessErrorHandlerTemplate(ProcessErrorEventArgs eventArgs)
        {
            Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
