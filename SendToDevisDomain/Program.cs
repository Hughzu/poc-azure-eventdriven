using Azure.Messaging.EventGrid;
using Azure;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

EventGridPublisherClient client = new EventGridPublisherClient(
    new Uri("https://devis-domain.westeurope-1.eventgrid.azure.net/api/events"),
    new AzureKeyCredential("CnXZ6JT4Iad9nSBcOHHkXbcM1WhUV+k7G8UNA2t4P7g="));

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

