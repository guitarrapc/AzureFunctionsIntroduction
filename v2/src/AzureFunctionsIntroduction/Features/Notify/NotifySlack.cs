using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Utf8Json;

namespace AzureFunctionsIntroduction.Notify
{
    public class NotifySlack : INotify
    {
        private static string _slackWebhookUrl = ConfigurationManagerHelper.GetOrDefault("SlackIncomingWebhookUrl");
        private static HttpClient client = new HttpClient();

        public async Task<HttpResponseMessage> SendAsync(string json)
        {
            var res = await client.PostAsync(_slackWebhookUrl, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("payload", json)
            }));
            return res;
        }
    }
}
