#r "Newtonsoft.Json"
#r "System.Configuration"
#load "..\EnumerableExtensions.csx"
#load "..\RescureUrl.csx"

using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    var result = data.result[0];
    
    // Debug
    log.Info($"result : {result}");
    log.Info($"data : {data}");

    if (result == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Seems result not contains in json."
        });
    }

    // Create Bot Response
    var content = CreateResponseContent(result, log);

    using (var client = new HttpClient())
    {
        // Set line authorization header
        client.DefaultRequestHeaders.Add("X-Line-ChannelID", ConfigurationManager.AppSettings["LineChannelId"]);
        client.DefaultRequestHeaders.Add("X-Line-ChannelSecret", ConfigurationManager.AppSettings["LineChannelSecret"]);
        client.DefaultRequestHeaders.Add("X-Line-Trusted-User-With-ACL", ConfigurationManager.AppSettings["LineMid"]);

        // Send Response to Line Sender
        var res = await client.PostAsJsonAsync("https://trialbot-api.line.me/v1/events",
            new {
                to = new[] { result.content.from },
                toChannel = "1383378250",
                // “138311609000106303”	Received message (example: text, images)
                // “138311609100106403”	Received operation (example: added as friend)
                eventType = "138311608800106203",
                content = content
            }
        );
        
        // Response Code
        return req.CreateResponse(res.StatusCode, new {
            text = $"{content.contentType}"
        });
    }
}

static Content CreateResponseContent(dynamic result, TraceWriter log)
{
    var text = result.content.text;
    var contentMetaData = result.content.contentMetadata;
    var location = result.content.location;
    Content content = null;

    if (location != null)
    {
        string latitude = location.latitude;
        string longitude = location.longitude;
        string address = location.address;
        log.Info($"location. Latitude : {latitude}, Longitude : {longitude}, Address : {address}");
        var responseText = new RescureUrl(latitude, longitude, address).GetAsync().Result;
        content = new Content
        {
            contentType = 1,
            toType = 1,
            text = responseText,
        };
    }
    else if (text != null)
    {
        var responseText = $"位置情報を共有してくれると緊急避難情報を調べます！";
        log.Info($"message : {responseText}");
        content = new Content
        {
            contentType = 1,
            toType = 1,
            text = responseText,
        };
    }
    else if (contentMetaData.SKTID != "")
    {
        var responseText = $"位置情報を共有してくれると緊急避難情報を調べます！";
        log.Info($"message : {responseText}");
        content = new Content
        {
            contentType = 1,
            toType = 1,
            text = responseText,
        };
    }
    return content;
}

public class Content
{
    public int contentType { get; set; }
    public int toType { get; set; }
    public string text { get; set; }
    public ContentMetadata contentMetadata { get; set; }
}

public class ContentMetadata
{
    public string STKID { get; set; }
    public string STKPKGID { get; set; }
    public string STKVER { get; set; }
}