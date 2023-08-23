using Shared;

const string connectionStringEH = "Endpoint=sb://poceventhubns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Nl86kAe9uXau5CeTvENKckMBTxofTmJ5k+AEhB+n1/w=";
const string connectionStringSA = "DefaultEndpointsProtocol=https;AccountName=merlinpoceventhubsa;AccountKey=l6/ILjHCWIAX2E4SD+PSef4VfgVtV74Zz7P3QW3n61rqVDJDSLdL6LojK7Y5+moEGbEJZlQNwqkL+AStAlsySQ==;EndpointSuffix=core.windows.net";

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
    ProducerConsumerFactory.Consumer(connectionStringSA, container, connectionStringEH, eventHub, "BillingWorker - BILLING QUEUE", 
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
    ProducerConsumerFactory.Consumer(connectionStringSA, estimatesContainer, connectionStringEH, estimatesEventHub, "BillingWorker - ESTIMATES QUEUE", 
        ProducerConsumerFactory.ProcessEventHandlerTemplate("BillingWorker - ESTIMATES QUEUE", () => ProducerConsumerFactory.Producer(connectionStringEH, eventHub, "BillingWorker - ESTIMATES QUEUE", e), interestingEvents), 
        ProducerConsumerFactory.ProcessErrorHandlerTemplate); ;
}).Start();

Thread.Sleep(Timeout.Infinite);