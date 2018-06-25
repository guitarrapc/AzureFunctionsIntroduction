using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureFunctionsIntroduction
{
    public static class EventGridWebhookCSharp
    {
        const string SubscriptionValidationEvent = "Microsoft.EventGrid.SubscriptionValidationEvent";
        const string StorageBlobCreatedEvent = "Microsoft.Storage.BlobCreated";

        [FunctionName("EventGridWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"C# HTTP trigger function for EventGrid begun");
            var response = string.Empty;

            var requestContent = await req.Content.ReadAsStringAsync();
            var eventGridEvents = JsonConvert.DeserializeObject<EventGridEvent[]>(requestContent);

            foreach (var eventGridEvent in eventGridEvents)
            {
                var dataObject = eventGridEvent.Data as JObject;

                // Deserialize the event data into the appropriate type based on event type
                if (string.Equals(eventGridEvent.EventType, SubscriptionValidationEvent, StringComparison.OrdinalIgnoreCase))
                {
                    var eventData = dataObject.ToObject<SubscriptionValidationEventData>();
                    log.Info($"Got SubscriptionValidation event data, validation code: {eventData.ValidationCode}, topic: {eventGridEvent.Topic}");

                    // Do any additional validation (as required) and then return back the below response
                    var responseData = new SubscriptionValidationResponse();
                    responseData.ValidationResponse = eventData.ValidationCode;
                    return req.CreateResponse(HttpStatusCode.OK, responseData);
                }
                else if (string.Equals(eventGridEvent.EventType, StorageBlobCreatedEvent, StringComparison.OrdinalIgnoreCase))
                {
                    var eventData = dataObject.ToObject<StorageBlobCreatedEventData>();
                    log.Info($"Got BlobCreated event data, blob URI {eventData.Url}");
                }

                log.Info($"=====Debug Message=====");
                log.Info($"Subject: {eventGridEvent.Subject}");
                log.Info($"Time: {eventGridEvent.EventTime}");
                log.Info($"Event data: {eventGridEvent.Data.ToString()}");
            }

            return req.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
