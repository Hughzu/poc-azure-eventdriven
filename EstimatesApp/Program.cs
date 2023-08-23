using MessageBusShared;

const string topicName = "estimates-topic";
const string subscriptionName = "EstimateApp";

// Send EstimatesSigned
Thread estimatesSender = new Thread(() =>
{
    while (true)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Type = MessageType.EstimatesSigned,
            Data = "Devis",
            MessageTime = DateTime.UtcNow,
            DataVersion = "1.0",
        };

        MessageBusHelper.Send(Keys.NamespaceConnectionString, topicName, message);

        Thread.Sleep(5000);
    }
});
estimatesSender.Start();

//Receive messages from Estimates Topic
Thread estimatesConsumer = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicName, subscriptionName, "EstimatesApp",
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("EstimatesApp", ConsoleColor.Yellow), MessageBusHelper.ErrorHandlerTemplate);

});
estimatesConsumer.Start();

//Receive messages from Estimates Topic Dead Letter Queue
Thread estimatesConsumerDeadLetterQueue = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicName, subscriptionName, "EstimatesApp-DLQ",
        MessageBusHelper.DeadLetterQueue, MessageBusHelper.MessageHandlerTemplate("EstimatesApp", ConsoleColor.DarkYellow), MessageBusHelper.ErrorHandlerTemplate);

});
estimatesConsumerDeadLetterQueue.Start();

Thread.Sleep(Timeout.Infinite);


