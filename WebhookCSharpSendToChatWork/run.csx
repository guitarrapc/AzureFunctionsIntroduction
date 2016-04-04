#r "Newtonsoft.Json"
#r "System.Threading.Tasks"

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using Chatwork.Service;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Verbose($"Webhook was triggered!");
    
    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.channel == 0 || data.text == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass channel/text properties in the input object"
        });
    }
    
    var chatworkApiKey = "INPUT YOUR API KEY HERE";
    int roomId = data.channel;
    string body = data.text;
    
    var client = new ChatworkClient(chatworkApiKey);
    await client.Room.SendMessgesAsync(roomId, body);

    return req.CreateResponse(HttpStatusCode.OK, new {
        RoomId = roomId,
        Message = body,
    });
}