#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Verbose($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    var result = data.result[0];
    
    // Debug
    log.Verbose($"result : {result}");

    if (result == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Seems result not contains in json."
        });
    }

    // Create Bot Response
    var content = CreateResponseContent(result, log);

    // Send Response to Line Sender
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Add("X-Line-ChannelID", "Channel IDを入力");
        client.DefaultRequestHeaders.Add("X-Line-ChannelSecret", "Channel Secret を入力");
        client.DefaultRequestHeaders.Add("X-Line-Trusted-User-With-ACL", "MID を入力");
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
    Content content = null;

    if (text != null)
    {
        log.Verbose($"message");
        content = new Content
        {
            contentType = 1,
            toType = 1,
            text = $"オウム返しするぞい！ : {text}",
        };
    }
    else if (contentMetaData.SKTID != "")
    {
        log.Verbose($"image");
        content = new Content
        {
            contentType = 8,
            toType = 1,
            contentMetadata = new ContentMetadata
            {
                STKID = "100",
                STKPKGID = "1",
                STKVER = "100",
            }
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