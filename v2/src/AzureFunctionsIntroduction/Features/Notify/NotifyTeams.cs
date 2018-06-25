using System;
using System.Net.Http;
using System.Threading.Tasks;
using Utf8Json;

namespace AzureFunctionsIntroduction.Notify
{
    public class NotifyTeams : INotify
    {
        private static string teamsWebhookUrl = Environment.GetEnvironmentVariable("TeamsIncomingWebhookUrl");
        private static HttpClient client = new HttpClient();

        public async Task<HttpResponseMessage> SendAsync(string json)
        {
            var stringContent = new StringContent(json);
            var res = await client.PostAsync(teamsWebhookUrl, stringContent);
            return res;
        }

        public static string ToJson<T>(T message)
        {
            return JsonSerializer.ToJsonString<T>(message);
        }

        /// <summary>
        /// Serialize to Json String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="urlname"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToJson(string title, string text, string urlname, string url)
        {
            var message = new TeamsMessage
            {
                title = title,
                text = text,
            };
            message.potentialAction = new Potentialaction[]
            {
                new Potentialaction()
                {
                    name = urlname,
                    target = new string[] { url },
                },
            };
            return JsonSerializer.ToJsonString<TeamsMessage>(message);
        }
    }
}
