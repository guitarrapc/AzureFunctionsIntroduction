using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFunctionsIntroduction
{
    public static class KeyVaultWebhookCSharp
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("KeyVaultWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            return await Execute(req);
        }

        private static async Task<HttpResponseMessage> Execute(HttpRequestMessage req)
        {
            switch (req.Method.Method)
            {
                case "POST":
                    return await PostHandler(req);
                case "GET":
                    return await GetHandler(req);
                default:
                    throw new NotImplementedException();
            }
        }

        private static async Task<HttpResponseMessage> PostHandler(HttpRequestMessage req)
        {
            string jsonContent = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Input>(jsonContent);

            // You can access Azure Functions Portal > Application Settings setting variable.
            var envKey = data.key;
            var secret = await GetKeyVaultSecret(envKey);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                key = envKey,
                value = secret,
            });
        }

        private static async Task<HttpResponseMessage> GetHandler(HttpRequestMessage req)
        {
            var input = new Input();
            var properties = typeof(Input).GetProperties().Select(x => x.Name).ToArray();
            var keyValues = req.GetQueryNameValuePairs().Where(x => properties.Contains(x.Key));
            if (!keyValues.Any()) throw new NullReferenceException("No query Parameter 'key' not found.");

            // only check first key
            var keyValue = keyValues.First();
            var property = typeof(Input).GetProperty(keyValue.Key);
            property.SetValue(input, keyValue.Value);

            // You can access Azure Functions Portal > Application Settings setting variable.
            var envKey = input.key;
            var secret = await GetKeyVaultSecret(envKey);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                key = envKey,
                value = secret,
            });
        }

        private static async Task<string> GetKeyVaultSecret(string envKey)
        {
            // Make sure AppService can access to KeyVault by
            // 1. Enable Managed Service Identity : AppService
            // 2. Enable AccessPolicy for AppService. : KeyVault
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback), client);
            var secret = (await kvClient.GetSecretAsync(envKey)).Value;
            return secret;
        }

        private class Input
        {
            public string key { get; set; }
        }
    }
}
