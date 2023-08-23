using MessageBusShared;

const string namespaceConnectionString = "Endpoint=sb://merlinservicebusns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mzC9N+S6Uvi8h3fDrLnj4XZ5gQxjpyTq1+ASbIYt984=";
const string topicName = "billing-topic";
const string subscriptionName = "BillingApp";

const string topicNameEstimates = "estimates-topic";

 
var billingConsumer = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicName, subscriptionName, "BillingApp-BillingConsumer", 
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("BillingApp-BillingConsumer", ConsoleColor.Cyan), MessageBusHelper.ErrorHandlerTemplate);

});
billingConsumer.Start();

var billingConsumerDeadLetterQueue = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicName, subscriptionName, "BillingApp-BillingConsumer-DLQ", 
        MessageBusHelper.DeadLetterQueue, MessageBusHelper.MessageHandlerTemplate("BillingApp-BillingConsumer", ConsoleColor.DarkCyan), MessageBusHelper.ErrorHandlerTemplate);

});
billingConsumerDeadLetterQueue.Start();

var estimatesConsumer = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicNameEstimates, subscriptionName, "BillingApp-EstimatesConsumer", 
        MessageBusHelper.StandardQueue, MessageBusHelper.MessageHandlerTemplate("BillingApp-EstimatesConsumer", ConsoleColor.Yellow, sendDeposit), MessageBusHelper.ErrorHandlerTemplate);

});
estimatesConsumer.Start();

var estimatesConsumerDeadLetterQueue = new Thread(() =>
{
    MessageBusHelper.Receive(namespaceConnectionString, topicNameEstimates, subscriptionName, "BillingApp-EstimatesConsumer-DQL", 
        MessageBusHelper.DeadLetterQueue, MessageBusHelper.MessageHandlerTemplate("BillingApp-EstimatesConsumer", ConsoleColor.DarkYellow, sendDeposit), MessageBusHelper.ErrorHandlerTemplate);

});
estimatesConsumerDeadLetterQueue.Start();



Thread.Sleep(Timeout.Infinite);


void sendDeposit()
{
    var message = new Message
    {
        Id = Guid.NewGuid(),
        Type = MessageType.BillingDepositSent,
        Data = "Deposit",
        MessageTime = DateTime.UtcNow,
        DataVersion = "1.0",
    };

    MessageBusHelper.Send(namespaceConnectionString, topicName, message);
}
