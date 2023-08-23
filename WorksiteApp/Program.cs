using MessageBusShared;

const string topicName = "worksite-topic";
const string subscriptionName = "WorksiteApp";

const string topicNameEstimates = "estimates-topic";


var worksiteConsumer = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicName, subscriptionName, "WorksiteApp-WorksiteConsumer", 
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("WorksiteApp-WorksiteConsumer", ConsoleColor.Green), MessageBusHelper.ErrorHandlerTemplate);

});
worksiteConsumer.Start();

var worksiteConsumerDeadLetterQueue = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicName, subscriptionName, "WorksiteApp-WorksiteConsumer-DLQ", 
        MessageBusHelper.DeadLetterQueue, MessageBusHelper.MessageHandlerTemplate("WorksiteApp-WorksiteConsumer", ConsoleColor.DarkGreen), MessageBusHelper.ErrorHandlerTemplate);

});
worksiteConsumerDeadLetterQueue.Start();

var estimatesConsumer = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicNameEstimates, subscriptionName, "WorksiteApp-EstimatesConsumer", 
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("WorksiteApp-EstimatesConsumer", ConsoleColor.Yellow, sendWorksiteCreated), MessageBusHelper.ErrorHandlerTemplate);

});
estimatesConsumer.Start();

var estimatesConsumerDeadLetterQueue = new Thread(() =>
{
    MessageBusHelper.Receive(Keys.NamespaceConnectionString, topicNameEstimates, subscriptionName, "WorksiteApp-EstimatesConsumer-DLQ", 
        MessageBusHelper.DeadLetterQueue, MessageBusHelper.MessageHandlerTemplate("WorksiteApp-EstimatesConsumer", ConsoleColor.DarkYellow, sendWorksiteCreated), MessageBusHelper.ErrorHandlerTemplate);

});
estimatesConsumerDeadLetterQueue.Start();



Thread.Sleep(Timeout.Infinite);


void sendWorksiteCreated()
{
    var message = new Message
    {
        Id = Guid.NewGuid(),
        Type = MessageType.WorksiteCreated,
        Data = "Worksite",
        MessageTime = DateTime.UtcNow,
        DataVersion = "1.0",
    };

    MessageBusHelper.Send(Keys.NamespaceConnectionString, topicName, message, ConsoleColor.Magenta);
}