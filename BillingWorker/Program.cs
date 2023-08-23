using Shared;

// Billing
const string eventHub = "billing-domain";
const string container = "billing-container";

// Estimates
const string estimatesEventHub = "estimates-domain";
const string estimatesContainer = "estimates-container";
var interestingEvents = new List<EventType>()
{
    EventType.EstimatesSigned
};


new Thread(() =>
{
    Thread.CurrentThread.IsBackground = true;
    ProducerConsumerFactory.Consumer(Keys.connectionStringSA, container, Keys.connectionStringEH, eventHub, "BillingWorker - BILLING QUEUE", 
        ProducerConsumerFactory.ProcessEventHandlerTemplate("BillingWorker - BILLING QUEUE"), 
        ProducerConsumerFactory.ProcessErrorHandlerTemplate);
}).Start();


var e = new Event()
{
    Id = Guid.NewGuid(),
    DataVersion = "1.0",
    EventTime = DateTime.UtcNow,
    Type = EventType.BillingDepositSent,
    Data = Guid.NewGuid(),
};
new Thread(() =>
{
    Thread.CurrentThread.IsBackground = true;
    ProducerConsumerFactory.Consumer(Keys.connectionStringSA, estimatesContainer, Keys.connectionStringEH, estimatesEventHub, "BillingWorker - ESTIMATES QUEUE", 
        ProducerConsumerFactory.ProcessEventHandlerTemplate("BillingWorker - ESTIMATES QUEUE", () => ProducerConsumerFactory.Producer(Keys.connectionStringEH, eventHub, "BillingWorker - ESTIMATES QUEUE", e), interestingEvents), 
        ProducerConsumerFactory.ProcessErrorHandlerTemplate); ;
}).Start();

Thread.Sleep(Timeout.Infinite);