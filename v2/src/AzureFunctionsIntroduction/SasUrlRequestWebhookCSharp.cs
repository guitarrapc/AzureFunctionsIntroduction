using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctionsIntroduction.Features;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsIntroduction
{
    public static class SasUrlRequestWebhookCSharp
    {
        private static HttpClient client = new HttpClient();
        private static KeyVaultClient kvClient = null;
        private static SecretBundle storageAccountConnectionStringBundle = null;

        [FunctionName("SasUrlRequestWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var request = await req.Content.ReadAsAsync<SasRequest>();

            // Run
            var response = await GetSasUrl(request);
            return req.CreateResponse<AssetBundleInfomationResponse>(HttpStatusCode.OK, response);
        }

        private static async Task<AssetBundleInfomationResponse> GetSasUrl(SasRequest request)
        {
            // TODO : key = assetName -> key reference to table storage and return assetname.
            var asset = GetBlobName(request?.Key);

            // KeyVault からBlob AccountのConnectionStringを取得
            if (kvClient == null || storageAccountConnectionStringBundle == null)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback), client);
                storageAccountConnectionStringBundle = await kvClient.GetSecretAsync(EnvironmentHelper.GetOrDefault(AppSettings.EnvKeyVaultSasBlobItemConnectionString, ""));
            }

            // リクエストごとにShortSAS Token の生成
            var sasTokenReqeust = new SasTokenRequest(storageAccountConnectionStringBundle.Value);
            var sasToken = await sasTokenReqeust.GenerateDefaultSasTokenAsync(EnvironmentHelper.GetOrDefault(AppSettings.EnvSasBlobItemContainer, ""), asset);
            var response = new AssetBundleInfomationResponse(new Uri(EnvironmentHelper.GetOrDefault(AppSettings.EnvSasBlobPrimaryEndpoint, "")), EnvironmentHelper.GetOrDefault(AppSettings.EnvSasBlobItemContainer, ""), asset, sasToken);

            return response;
        }

        private static string GetBlobName(string key)
        {
            // temporary just test item
            return string.IsNullOrWhiteSpace(key)
                ? EnvironmentHelper.GetOrDefault(AppSettings.EnvSasBlobItemName, "")
                : EnvironmentHelper.GetOrDefault(AppSettings.EnvSasBlobItemName, "");
        }
    }

    public class SasRequest
    {
        public string Key { get; set; }
    }

    public class AssetBundleInfomationResponse
    {
        public string Id => Guid.NewGuid().ToString();
        public string SasUrl { get; }
        public string AssetName { get; }

        public AssetBundleInfomationResponse(Uri baseUrl, string container, string assetName, string sasToken)
        {
            SasUrl = new Uri(baseUrl, $"{container}/{assetName}") + sasToken;
            AssetName = assetName;
        }
    }
}
