using MessageBusShared;

const string topicName = "auto-complete-topic";
const string subscriptionName = "AutoCompleteApp";


var sender = new Thread(() =>
{
    var message = new Message
    {
        Id = Guid.NewGuid(),
        Type = MessageType.Unknown,
        Data = "TestAutoComplete",
        MessageTime = DateTime.UtcNow,
        DataVersion = "1.0",
    };

    for (var i = 0; i < 10; i++)
    {
        MessageBusHelper.Send(Keys.NamespaceConnectionString, topicName, message, ConsoleColor.Magenta);
        Thread.Sleep(500);
    }
});
// sender.Start();


//Receive messages from AutoComplete Topic
var receiver = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicName, subscriptionName, "AutoCompleteApp",
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("AutoCompleteApp", ConsoleColor.Yellow/*, waitUntilExpiration*/),
        MessageBusHelper.ErrorHandlerTemplate);
});
receiver.Start();

//Receive messages from AutoComplete Topic Dead Letter Queue
var receiverDLQ = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicName, subscriptionName, "AutoCompleteApp-DLQ",
        MessageBusHelper.DeadLetterQueue,
        MessageBusHelper.MessageHandlerTemplate("AutoCompleteApp", ConsoleColor.DarkYellow),
        MessageBusHelper.ErrorHandlerTemplate);
});
receiverDLQ.Start();

Thread.Sleep(Timeout.Infinite);
return;

void waitUntilExpiration()
{
    Console.WriteLine("Start sleeping ...");
    Thread.Sleep(10000);
    Console.WriteLine("Finished sleeping ...");
}