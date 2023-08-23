using Azure.Messaging.EventGrid;
using Azure;
using System.Globalization;
using Shared_Poc_1;

EventGridPublisherClient client = new EventGridPublisherClient(
    new Uri(Keys.endpoint),
    new AzureKeyCredential(Keys.keyCredential));

// Add EventGridEvents to a list to publish to the topic
EventGridEvent egEvent =
    new EventGridEvent(
        "ExampleEventSubject",
        "Example.EventType",
        "1.0",
        $"This is the event data {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff.ffffff", CultureInfo.InvariantCulture)} ");
egEvent.Topic = "devis-domain";


// Send the event
await client.SendEventAsync(egEvent);

