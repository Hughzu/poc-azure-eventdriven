using Azure.Messaging.EventHubs.Producer;

var producerClient = new EventHubProducerClient(
    "Endpoint=sb://poceventhubns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Nl86kAe9uXau5CeTvENKckMBTxofTmJ5k+AEhB+n1/w=",
    "devis-eh");



Producer();
Consumer();







void Producer()
{

}

void Consumer()
{

}