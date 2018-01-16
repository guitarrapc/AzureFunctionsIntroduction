using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Text.RegularExpressions;

namespace AzureFunctionsIntroduction
{
    public static class LineBotWebhookCSharp
    {
        private static readonly string lineChannelId = Environment.GetEnvironmentVariable("LineChannelId");
        private static readonly string lineChannelSecret = Environment.GetEnvironmentVariable("LineChannelSecret");
        private static readonly string lineMid = Environment.GetEnvironmentVariable("LineMid");

        [FunctionName("LineBotWebhookCSharp")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"LineBotWebhookCSharp : Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);
            var result = data.result[0];

            // Debug
            log.Info($"result : {result}");
            log.Info($"data : {data}");

            if (result == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Seems result not contains in json."
                });
            }

            // Create Bot Response
            var content = CreateResponseContent(result, log);

            using (var client = new HttpClient())
            {
                // Set line authorization header
                client.DefaultRequestHeaders.Add("X-Line-ChannelID", lineChannelId);
                client.DefaultRequestHeaders.Add("X-Line-ChannelSecret", lineChannelSecret);
                client.DefaultRequestHeaders.Add("X-Line-Trusted-User-With-ACL", lineMid);

                // Send Response to Line Sender
                var res = await client.PostAsJsonAsync("https://trialbot-api.line.me/v1/events",
                    new
                    {
                        to = new[] { result.content.from },
                        toChannel = "1383378250",
                        // “138311609000106303”	Received message (example: text, images)
                        // “138311609100106403”	Received operation (example: added as friend)
                        eventType = "138311608800106203",
                        content = content
                    }
                );

                // Response Code
                return req.CreateResponse(res.StatusCode, new
                {
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
                var responseText = "位置情報を共有してくれると緊急避難情報を調べます！";
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
                var responseText = "位置情報を共有してくれると緊急避難情報を調べます！";
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

        public class RescureUrl
        {
            // 「近くの緊急避難所を探します」を利用 : だいたい半径3km以内で近くの避難所を教えてくれます
            private static readonly string _searchBaseUrl = "https://0312.yanoshin.jp/rescue/index";

            // 「ナビタイム災害情報」を利用 : kumamoto prefecture が含まれていたら熊本地震とみなしてURL 追加
            private static readonly string _navitimeUrl = "http://www.navitime.co.jp/saigai/?from=pctop";

            // Google クライシスレスポンスを利用 : https://www.google.org/crisisresponse/japan
            // Map 表示もできるけど、位置関係なしなので一旦TOP のみでMapはなし : https://www.google.org/crisisresponse/japan/maps?hl=ja
            private static readonly string _googleChrisisUrl = "https://www.google.org/crisisresponse/japan";

            public string Latitude { get; private set; }
            public string Longitude { get; private set; }
            public string Address { get; private set; }
            public bool IsKumamoto => IsKumamotoIncluded(this.Address);

            public RescureUrl(string latitude, string longitude, string address)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
                this.Address = address;
            }

            public async Task<string> GetAsync()
            {
                var endMessage = new[] {$"送っていただいた現在地情報は次の通りです。",
            $"経度 : {this.Latitude}",
            $"緯度 : {this.Longitude}",
            $"住所 : {this.Address}",
        }.ToJoinedString(Environment.NewLine);
                var requestUrl = new Uri($"{_searchBaseUrl}/{this.Latitude}/{this.Longitude}");
                using (var client = new HttpClient())
                {
                    var res = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
                    if (res.IsSuccessStatusCode)
                    {
                        string postMessage = endMessage;
                        if (this.IsKumamoto)
                        {
                            // 熊本県の住所が入っていたら差し込む
                            postMessage = new[] {"熊本県の方ですか？「ナビタイム災害情報」もご参考にどうぞ",
                        _navitimeUrl,
                        $"{Environment.NewLine}",
                        endMessage,
                    }
                            .ToJoinedString(Environment.NewLine);
                        }

                        var message = new[] {@"「近くの緊急避難所を探します」で、検索結果が見つかりました。",
                    $"最寄りの避難所情報 URL : {requestUrl.AbsoluteUri}",
                    Environment.NewLine,
                    $"安否情報はGoogleクライシスレスポンスもどうぞ URL : {_googleChrisisUrl}",
                    Environment.NewLine,
                    postMessage,
                }.ToJoinedString(Environment.NewLine);

                        return message;
                    }
                    return $"指定した住所がみつかりませんでした。もう一度試していただけますか？{Environment.NewLine}{endMessage}";
                }
            }

            public static bool IsKumamotoIncluded(string address)
            {
                var isEngCultureInvaliant = Regex.IsMatch(address, @"Kumamoto\s*Prefecture", RegexOptions.CultureInvariant);
                var isEngIgnoreCase = Regex.IsMatch(address, @"Kumamoto\s*Prefecture", RegexOptions.IgnoreCase);
                var isEngIgnoreWhitespace = Regex.IsMatch(address, @"Kumamoto\s*Prefecture", RegexOptions.IgnorePatternWhitespace);
                var isJapaneseCultureInvaliat = Regex.IsMatch(address, "熊本県", RegexOptions.CultureInvariant);
                return isEngCultureInvaliant || isEngIgnoreCase || isEngIgnoreWhitespace || isJapaneseCultureInvaliat;
            }
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
    }
}
