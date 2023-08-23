using MessageBusShared;

const string namespaceConnectionString = "Endpoint=sb://merlinservicebusns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mzC9N+S6Uvi8h3fDrLnj4XZ5gQxjpyTq1+ASbIYt984=";
const string topicName = "worksite-topic";
const string subscriptionName = "WorksiteApp";

const string topicNameEstimates = "estimates-topic";


var worksiteConsumer = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicName, subscriptionName, "WorksiteApp-WorksiteConsumer", 
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("WorksiteApp-WorksiteConsumer", ConsoleColor.Green), MessageBusHelper.ErrorHandlerTemplate);

});
worksiteConsumer.Start();

var worksiteConsumerDeadLetterQueue = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicName, subscriptionName, "WorksiteApp-WorksiteConsumer-DLQ", 
        MessageBusHelper.DeadLetterQueue, MessageBusHelper.MessageHandlerTemplate("WorksiteApp-WorksiteConsumer", ConsoleColor.DarkGreen), MessageBusHelper.ErrorHandlerTemplate);

});
worksiteConsumerDeadLetterQueue.Start();

var estimatesConsumer = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicNameEstimates, subscriptionName, "BillingApp-EstimatesConsumer", 
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("BillingApp-EstimatesConsumer", ConsoleColor.Yellow, sendWorksiteCreated), MessageBusHelper.ErrorHandlerTemplate);

});
estimatesConsumer.Start();

var estimatesConsumerDeadLetterQueue = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicNameEstimates, subscriptionName, "BillingApp-EstimatesConsumer-DLQ", 
        MessageBusHelper.DeadLetterQueue, MessageBusHelper.MessageHandlerTemplate("BillingApp-EstimatesConsumer", ConsoleColor.DarkYellow, sendWorksiteCreated), MessageBusHelper.ErrorHandlerTemplate);

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

    MessageBusHelper.Send(namespaceConnectionString, topicName, message);
}