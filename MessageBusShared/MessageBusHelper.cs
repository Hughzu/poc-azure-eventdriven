using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace MessageBusShared
{
    public static class MessageBusHelper
    {
        public static ServiceBusProcessorOptions StandardQueue = new()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            AutoCompleteMessages = false, // abandonned the message if not explicitly completed -> pas l'air de fonctionner ..
        };
        
        public static ServiceBusProcessorOptions DeadLetterQueue = new()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            AutoCompleteMessages = false, // abandonned the message if not explicitly completed -> pas l'air de fonctionner ..
            SubQueue = SubQueue.DeadLetter
        };
        
        public static async void Send(string namespaceConnectionString, string topicName, Message message)
        {
            ServiceBusClient client = new ServiceBusClient(namespaceConnectionString); ;
            ServiceBusSender sender = client.CreateSender(topicName);

            try
            {
                var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)))
                {
                    CorrelationId = message.Type.ToString(),
                    Subject = message.Id.ToString(),
                    ApplicationProperties =
                {
                    { "id", message.Id },
                    { "type", (int)message.Type },
                    { "data", message.Data },
                    { "messagetime", message.MessageTime },
                    { "dataversion", message.DataVersion },

                }
                };
                await sender.SendMessageAsync(serviceBusMessage);
                
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Sent message with Id={0}, Type={1}, Data={2}", message.Id, message.Type, message.Data);
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        public static async void Receive(string namespaceConnectionString, string topicName, string subscriptionName, string appName,
            ServiceBusProcessorOptions serviceBusProcessorOptions, Func<ProcessMessageEventArgs, Task> messageHandler, Func<ProcessErrorEventArgs, Task> errorHandler)
        {
            var client = new ServiceBusClient(namespaceConnectionString);
            var processor = client.CreateProcessor(topicName, subscriptionName, serviceBusProcessorOptions);

            try
            {
                processor.ProcessMessageAsync += messageHandler;
                processor.ProcessErrorAsync += errorHandler;

                Console.WriteLine($"{appName} - Start receiving ...");
                await processor.StartProcessingAsync();
                Thread.Sleep(Timeout.Infinite);
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        public static Func<ProcessMessageEventArgs, Task> MessageHandlerTemplate(string appName, ConsoleColor consoleColor, Action? method = null)
        {
            return async (ProcessMessageEventArgs args) =>
            {
                try
                {
                    var body = args.Message.Body.ToString();
                
                    Console.ForegroundColor = consoleColor;
                    Console.WriteLine($"[{appName}] Received: {body}");

                    method?.Invoke();

                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    // await args.DeadLetterMessageAsync(args.Message); //to use if the format is not correct ? 
                    await args.AbandonMessageAsync(args.Message);
                }
                
            };
        }

        public static Task ErrorHandlerTemplate(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}